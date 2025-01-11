# SteamTokenDump

A C# console application designed for extracting Steam refresh tokens from memory. This project was heavily inspired by [LimerBoy's SteamTokenDump](https://github.com/LimerBoy/SteamTokenDump). Unlike the original implementation, this version is optimized and written in .NET 9, providing improved performance and maintainability over the original .NET Framework 4.7.2-based implementation.

---

## Features

- **Process Memory Scanning**: Reads and analyzes the memory of Steam processes to extract valid refresh tokens.
- **Regex-based Token Matching**: Uses compiled regular expressions to efficiently identify token patterns.
- **Streamlined Logging**: Offers categorized logs for warnings, errors, successes, and informational messages.
- **Optimized for .NET 9**: Improved performance and compatibility with modern .NET runtime.

---

## How It Works

1. The application scans the memory of running Steam processes.
2. Matches token patterns using a predefined regular expression.
3. Outputs any discovered refresh tokens to the console.

---

## Usage

1. Clone the repository:

   ```bash
   git clone https://github.com/XeinTDM/SteamTokenDump.git
   cd SteamTokenDump
   ```

2. Build the project:

   ```bash
   dotnet build
   ```

3. Run the executable:

   ```bash
   dotnet run --project ./SteamTokenDump.csproj
   ```

---

## Requirements

- .NET 9 Runtime or SDK
- Windows operating system (leverages Windows-specific APIs)

---

## Disclaimer

This tool is provided **as is** for educational purposes only. The author assumes no responsibility for any misuse or unintended consequences arising from the use of this application. Use at your own risk.

---

## License

This project is licensed under the [The Unlicense](LICENSE), granting you the freedom to use, modify, and distribute the code as you see fit.

---

## Acknowledgements

Special thanks to [LimerBoy](https://github.com/LimerBoy) for the inspiration and groundwork laid by the original [SteamTokenDump](https://github.com/LimerBoy/SteamTokenDump) project.
