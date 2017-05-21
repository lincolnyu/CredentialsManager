using System;
using CredentialsManagerLib.Models;
using System.IO;
using System.Linq;

namespace CrendentialsConsole
{
    class Program
    {
        private Program(string repoPath)
        {
            _repoPath = repoPath;
            _repo = new Repository();
            TryLoad();
        }

        private string _repoPath;
        private Repository _repo;
        private bool _dirty;

        static void Main(string[] args)
        {
            var argIndex = Array.IndexOf(args, "-r");
            string repoPath;
            if (argIndex >= 0 && argIndex < args.Length - 1)
            {
                repoPath = args[argIndex + 1];
            }
            else
            {
                PrintUsage();
                return;
            }
            

            var prog = new Program(repoPath);
            prog.TryLoad();
            prog.RunLoop();
            prog.Save();
          
        }

        private void TryLoad()
        {
            if (File.Exists(_repoPath))
            {
                using (var sr = new StreamReader(_repoPath))
                {
                    var s = sr.ReadToEnd();
                    _repo.LoadFromJsonString(s);
                }
            }
        }

        private void Save()
        {
            if (_dirty)
            {
                using (var sw = new StreamWriter(_repoPath))
                {
                    var s = _repo.SaveToFormattedJsonString(0, 2);
                    sw.Write(s);
                }
                _dirty = false;
            }
        }

        private void RunLoop()
        {
            var run = true;
            while (run)
            {
                Console.Write(">");
                var cmd = Console.ReadLine();
                if (string.IsNullOrEmpty(cmd)) continue;
                switch (cmd)
                {
                    case "quit":
                        run = false;
                        continue;
                    case "help":
                        PrintLoopCommands();
                        continue;
                }
                var cmdsplit = cmd.Split(' ');
                if (cmdsplit.Length != 2)
                {
                    Console.WriteLine("Unrecognized command. 'help' for help");
                    continue;
                }
                var cmd1 = cmdsplit[0].ToLower();
                var cmd2 = cmdsplit[1].ToLower();
                if (cmd1 == "list" && cmd2 == "providers")
                {
                    ListProviders();
                    continue;
                }
                if (cmd2 == "provider")
                {
                    if (cmd1 == "add")
                    {
                        AddProvider();
                        continue;
                    }
                    else if (cmd1 == "remove")
                    {
                        RemoveProvider();
                        continue;
                    }
                    else if (cmd1 == "edit")
                    {
                        EditProvider();
                        continue;
                    }
                }
                else if (cmd2 == "account")
                {
                    if (cmd1 == "add")
                    {
                        AddAccount();
                        continue;
                    }
                    else if (cmd1 == "remove")
                    {
                        RemoveAccount();
                        continue;
                    }
                    else if (cmd1 == "edit")
                    {
                        EditAccount();
                        continue;
                    }
                }
                Console.WriteLine("Unrecognized command. 'help' for help");
            }
        }

        private void ListProviders()
        {
            foreach (var kvp in _repo.Providers)
            {
                Console.WriteLine($" {kvp.Key}");
            }
        }

        private void AddProvider()
        {
            var name = GetInput("Provider name:");
            var provider = new Provider();
            _repo.Providers.Add(name, provider);
        }

        private void RemoveProvider()
        {
            var name = GetInput("Provider name:");
            if (_repo.Providers.ContainsKey(name))
            {
                if (PromptConfirm())
                {
                    var found = _repo.Providers.Remove(name);
                    if (found)
                    {
                        Console.WriteLine("Successfully removed.");
                        _dirty = true;
                    }
                    else
                    {
                        Console.WriteLine("Failed to remove.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Provider not found.");
            }
        }

        private void EditProvider()
        {
            var name = GetInput("Provider name:");
            if (_repo.Providers.TryGetValue(name, out var provider))
            {
                if (PromptConfirm(new [] { 'Y', 'y' }, "Change name?"))
                {
                    var newName = GetInput("New name:");
                    if (PromptConfirm())
                    {
                        _repo.Providers.Remove(name);
                        _repo.Providers[newName] = provider;
                        _dirty = true;
                    }
                }
            }
            else
            {
                Console.WriteLine("Provider not found.");
            }
        }

        private void AddAccount()
        {
            var providerName = GetInput("Provider name:");
            if (_repo.Providers.TryGetValue(providerName, out var provider))
            {
                var inputName = GetInput("Input name:");
                if (!provider.Accounts.ContainsKey(inputName))
                {
                    var password = GetInput("Password:");
                    var account = new Account
                    {
                        UserName = inputName,
                        Password = password
                    };
                    provider.Accounts.Add(inputName, account);
                    _dirty = true;
                }
                else
                {
                    Console.WriteLine("Account already exists");
                }
            }
            else
            {
                Console.WriteLine("Provider not found.");
            }
        }

        private void RemoveAccount()
        {
            var providerName = GetInput("Provider name:");
            if (_repo.Providers.TryGetValue(providerName, out var provider))
            {
                var accountName = GetInput("Account name:");
                if (provider.Accounts.ContainsKey(accountName))
                {
                    if (PromptConfirm())
                    {
                        provider.Accounts.Remove(accountName);
                        _dirty = true;
                    }
                }
                else
                {
                    Console.WriteLine("Account not found.");
                }
            }
            else
            {
                Console.WriteLine("Provider not found.");
            }
        }

        private void EditAccount()
        {
            throw new System.NotImplementedException();
        }

        private bool PromptConfirm(string msg = "Are you sure? (Y)")
            => PromptConfirm(new char[] { 'Y' }, msg);

        private bool PromptConfirm(char[] yesKeys, string msg = "Are you sure?")
        {
            Console.Write(msg);
            var key = Console.ReadKey();
            Console.WriteLine();
            return yesKeys.Contains(key.KeyChar);
        }

        private string GetInput(string msg)
        {
            Console.Write(msg);
            return Console.ReadLine().Trim();
        }

        private static void PrintUsage()
        {
            Console.WriteLine("credentials -r <repo file path>");
        }

        private void PrintLoopCommands()
        {
            Console.WriteLine(" list providers");
            Console.WriteLine(" add/remove/edit provider/account");
            Console.WriteLine(" quit");
        }
    }
}
