using System.Configuration;
using System.Globalization;
using System.Xml.Linq;
using System.Xml;
using Theunis.Assesment.Web.Models;

namespace Theunis.Assesment.Web.Common
{
    public class CsvFileUtilities
    {
        public async Task<List<TransactionViewModel>> ValidateCsvDocumentAsync(IFormFile file, string path)
        {
            var transactions = new List<TransactionViewModel>();
            if (file == null) return transactions;

            string fileName = file.FileName;
            string filePath = Path.Combine(path, fileName);

            try
            {
                // Save the file asynchronously
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Read and process the CSV file
                using var sr = new StreamReader(filePath);
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    var columns = line.Split(',');

                    // Validate CSV row data
                    if (!ValidateCsvRow(columns))
                    {
                        Utilities.CreateLog($"Invalid data in row: {line}");
                        continue;
                    }

                    // Parse the CSV row into a TransactionViewModel
                    var transaction = new TransactionViewModel
                    {
                        TransactionId = columns[0],
                        Status = columns[4],
                        TransactionDate = ParseCsvDateTime(columns[3].Trim()),
                        Amount = decimal.Parse(columns[1]),
                        CurrencyCode = columns[2]
                    };

                    transactions.Add(transaction);
                }

                return transactions;
            }
            catch (Exception ex)
            {
                // General exception handling with logging
                Utilities.CreateLog($"Error processing CSV file {fileName}: {ex.Message}");
                throw;
            }
        }

        private bool ValidateCsvRow(string[] columns)
        {
            if (columns.Length < 5) return false;

            if (string.IsNullOrWhiteSpace(columns[0]) || columns[0].Length > 50) return false;
            if (!decimal.TryParse(columns[1], out _)) return false;
            if (string.IsNullOrWhiteSpace(columns[2])) return false;
            if (!DateTime.TryParseExact(columns[3], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)) return false;
            if (string.IsNullOrWhiteSpace(columns[4])) return false;

            return true;
        }

        private DateTime ParseCsvDateTime(string dateTimeStr)
        {
            const string format = "dd/MM/yyyy HH:mm:ss";
            if (DateTime.TryParseExact(dateTimeStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            {
                return dateTime;
            }
            throw new FormatException($"Invalid date format: {dateTimeStr}");
        }

    }
}
