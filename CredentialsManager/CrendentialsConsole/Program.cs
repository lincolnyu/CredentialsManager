using System;
using CredentialsManagerLib.Models;
using System.IO;

namespace CrendentialsConsole
{
    class Program
    {
        private Program(Repository repo)
        {
            _repo = repo;
        }

        private Repository _repo;

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

            var repo = new Repository();
            if (File.Exists(repoPath))
            {
                using (var sr = new StreamReader(repoPath))
                {
                    var s = sr.ReadToEnd();
                    repo.LoadFromJsonString(s);
                }
            }

            var prog = new Program(repo);
            prog.RunLoop();

            using (var sw = new StreamWriter(repoPath))
            {
                var s = repo.SaveToFormattedJsonString(0, 2);
                sw.Write(s);
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
            Console.Write("Provider name:");
            var input = Console.ReadLine().Trim();
            var provider = new Provider();
            _repo.Providers.Add(input, provider);
        }

        private void RemoveProvider()
        {
            Console.Write("Provider name:");
            var name = Console.ReadLine().Trim();
            if (_repo.Providers.ContainsKey(name))
            {
                Console.WriteLine("Are you sure?");
                var key = Console.ReadKey();
                Console.WriteLine();
                if (key.KeyChar == 'Y')
                {
                    var found = _repo.Providers.Remove(name);
                    if (found)
                    {
                        Console.WriteLine("Successfully removed.");
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
            Console.Write("Provider name:");
            var name = Console.ReadLine().Trim();
            if (_repo.Providers.TryGetValue(name, out var provider))
            {
                Console.Write("Change name?");
                var key = Console.ReadKey();
                Console.WriteLine();
                if (key.KeyChar == 'Y')
                {
                    Console.Write("New name:");
                    var newName = Console.ReadLine().Trim();
                    _repo.Providers.Remove(name);
                    _repo.Providers[newName] = provider;
                }
            }
            else
            {
                Console.WriteLine("Provider not found.");
            }
        }


        private void AddAccount()
        {

        }

        private void RemoveAccount()
        {

        }

        private void EditAccount()
        {

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
