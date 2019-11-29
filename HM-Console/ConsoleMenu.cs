using System;
using System.Text.RegularExpressions;
using ClassLibHejMorsan;

namespace ClassLibHejMorsan
{
    class ConsoleMenu
    {
        CountDown newCountdown = new CountDown();
        // ErrorHandling err = new ErrorHandling();

        //The Menu type is bool because we only want to switch to a new day when we explicitly says so.
        public bool StartMenu(Person P)
        {
            string userInput = "";

            while (true)
            {
                int Statementvalue = 0;
                int typeOfInserttoDB = 0;
                System.Console.WriteLine("");
                System.Console.WriteLine("[1] Add a new Person");
                System.Console.WriteLine("[2] Update Person");
                System.Console.WriteLine("[3] Delete Person");
                System.Console.WriteLine("[4] Quit");
                System.Console.WriteLine("Make a choice or press any key to skip to next day...");

                userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        {
                            //This variable decides which type of insert to DB we are doing, 1= addperson
                            typeOfInserttoDB = 1;
                            AddorUpdatePerson(P, Statementvalue, typeOfInserttoDB);
                            // refreshes person list
                            P.GetPersons();
                            break;
                        }
                    case "2":
                        {
                            //Statementvalue 2 means that we are adding an option to chooose which ID to change
                            Statementvalue = 2;
                            // typeofinserttoDB = 2 means that we are updating a person instead of adding.
                            typeOfInserttoDB = 2;
                            AddorUpdatePerson(P, Statementvalue, typeOfInserttoDB);
                            P.GetPersons();
                            break;
                        }
                    case "3":
                        {
                            DeletePerson(P);
                            P.GetPersons();
                            break;
                        }
                    // quit program
                    case "4":
                        return false;

                    default:
                        return true;
                }
            }
        }

        //The daily loop fetches the saved database, runs through it, person by person.
        //Gives the user the option to call if the counter has reached zero.
        //Otherwise triggers an overduecounter to display how many days overdue the user is.
        public void DailyLoop(Person P)
        {
            // load persons from DB
            P.GetPersons();
            // print person's details
            PrintAllPersonsWithoutId(P);
            foreach (var person in P.myPersons)
            {
                bool waitForCorrectInput = true;

                if (newCountdown.TimeToCallMom(person) == true)
                {
                    string input = "";
                    while (waitForCorrectInput)
                    {
                        Console.WriteLine($"Do you want to call {person.Name}? \n[Y]es or [N]o: ");
                        input = Console.ReadLine();
                        string userInput = input.ToUpper();

                        switch (userInput)
                        {
                            case "Y":
                                newCountdown.MomHasBeenCalled(person);
                                waitForCorrectInput = false;
                                break;

                            case "N":
                                Console.WriteLine($"You chose not to call {person.Name}.");
                                waitForCorrectInput = false;
                                continue;

                            default:
                                Console.WriteLine("Please write Y or N!");
                                Console.ReadLine();
                                break;
                        }
                    }
                }
                person.UpdateCounter(person.Id, person.CountDownTick);
            }
        }

        // deletes a person from DB
        private void DeletePerson(Person P)
        {
            int inputAsInt = 0;
            bool validId = false;

            while (true)
            {
                PrintAllPersonsWithId(P);

                while (!validId)
                {
                    // ask for person's id to be deleted
                    System.Console.Write("Enter ID of person to delete: ");
                    // try to get a number
                    string input = Console.ReadLine();
                    inputAsInt = 0;
                    // Send the value to errorhandling to try/catch it
                    if (input.inputIsInt())
                    {
                        inputAsInt = Convert.ToInt32(input);
                        validId = true;
                    }
                    else
                    {
                        System.Console.WriteLine("Enter a number!");
                    }
                }

                // if user exists in DB
                if (DB.FindUserInDB(inputAsInt, P))
                {
                    System.Console.WriteLine($"Are you sure you want to delete ID: {inputAsInt}");
                    System.Console.WriteLine($"[Y]es/[N]o");

                    string input = Console.ReadLine().ToUpper();

                    // check user confirmation
                    while (true)
                    {
                        switch (input)
                        {
                            case "Y":
                                {
                                    System.Console.WriteLine("Person has been deleted. Press any key...");
                                    P.DeletePerson(inputAsInt);
                                    Console.ReadLine();
                                    PrintAllPersonsWithoutId(P);
                                    return;
                                }
                            case "N":
                                {
                                    System.Console.WriteLine("No one has been deleted. Press any key...");
                                    PrintAllPersonsWithoutId(P);
                                    Console.ReadLine();
                                    return;
                                }
                            default:
                                {
                                    System.Console.WriteLine("Wrong input.");
                                    PrintAllPersonsWithoutId(P);
                                    Console.ReadLine();
                                    break;
                                }
                        }
                        break;
                    }
                }

                else
                {
                    Console.WriteLine("Id does not exist! Press any key...");
                    validId = false;
                    Console.ReadLine();
                }
            }
        }

        //Adds or Updates a person depending on the values set in the menu, to the database
        private void AddorUpdatePerson(Person P, int Statementvalue, int typeOfInserttoDB)
        {
            string name = "";
            string phone = "";
            string birthday = "";
            int counter = 0;
            int idToUpdate = 0;
            bool matchinDB = true;
            bool enteredInt = false;
            bool inputIsNoInt = true;
            bool majorLoop = true;

            if (Statementvalue == 2)
            {

                while (majorLoop)
                {
                    PrintAllPersonsWithId(P);

                    while (!enteredInt)
                    {
                        // ask for person's id to be deleted
                        System.Console.Write("Enter ID of person to update: ");
                        // try to get a number
                        try
                        {
                            idToUpdate = Convert.ToInt32(Console.ReadLine());
                            enteredInt = true;
                        }
                        catch
                        {
                            System.Console.WriteLine("Enter a number!");

                        }
                    }
                    if (DB.FindUserInDB(idToUpdate, P))
                    {
                        matchinDB = true;
                        majorLoop = false;
                    }

                    else
                    {
                        matchinDB = false;
                    }
                }
                if (!matchinDB)
                {
                    Console.WriteLine("Id does not exist! Press any key...");
                    enteredInt = false;
                    Console.ReadLine();
                }
            }

            while (inputIsNoInt)
            {
                System.Console.Write("Enter Name: ");
                name = Console.ReadLine();
                if (name.Length <= 50 && name.Length > 1)
                {
                    inputIsNoInt = false;
                }
                else
                {
                    Console.WriteLine("Enter between 2 and 50 characters!");
                }
            }

            // https://stackoverflow.com/questions/8764827/c-sharp-regex-validation-rule-using-regex-match
            // https://regex101.com/r/kF1uH5/2
            var regex = @"[0-9]{2,4}-[0-9]{2,3}\s[0-9]{2,3}\s[0-9]{2,3}$";
            inputIsNoInt = true;
            while (inputIsNoInt)
            {
                System.Console.Write("Enter Telephone number: ");
                phone = Console.ReadLine();
                var match = Regex.Match(phone, regex, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    inputIsNoInt = false;
                }
                else
                {
                    Console.WriteLine("Valid format is: 99-999 999 99, 99-999 99 99, 999-99 99 99");
                }
            }
// xxx
            // https://www.regular-expressions.info/dates.html
            regex = @"^(19|20)\d\d[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])$";

            inputIsNoInt = true;
            while (inputIsNoInt)
            {
                System.Console.Write("Enter Birthday (YYYY-MM-DD): ");
                birthday = Console.ReadLine();



                var match = Regex.Match(birthday, regex, RegexOptions.IgnoreCase);
                // if format matches a date AND date is in the past
                if (match.Success && DateTime.Now.CompareTo(DateTime.Parse(birthday)) > 0)
                {
                    inputIsNoInt = false;
                }

                else
                {
                    Console.WriteLine("Enter a valid date, format YYYY-MM-DD)!");
                }
            }

            // check for int
            inputIsNoInt = true;
            while (inputIsNoInt)
            {
                System.Console.Write("Enter the time interval (max 365): ");

                string input = Console.ReadLine();

                if (input.inputIsInt() && counter <= 365)
                {
                    counter = Convert.ToInt32(input);
                    inputIsNoInt = false;
                }
                else
                {
                    Console.WriteLine("Enter a number of max 365 days!");
                    inputIsNoInt = true;
                }
            }

            switch (typeOfInserttoDB)
            {
                case 1:
                    {
                        // add person to DB
                        P.AddPerson(name, phone.ToString(), birthday.ToString(), counter);
                        System.Console.WriteLine($"{name} has been added to the list. Press any key...");
                        Console.ReadLine();
                    }
                    break;
                case 2:
                    {
                        // update person
                        P.UpdatePerson(idToUpdate, name, phone, birthday, counter);
                        System.Console.WriteLine($"{name} has been updated. Press any key...");
                    }
                    break;
            }
        }

        private void PrintAllPersonsWithId(Person P)
        {
            foreach (var person in P.myPersons)
            {
                System.Console.WriteLine($"Id: {person.Id} {person.Name}");
            }
        }

        // prints all the persons saved in DB
        private static void PrintAllPersonsWithoutId(Person P)
        {
            foreach (var person in P.myPersons)
            {
                Console.WriteLine($"Namn: {person.Name}\tFödelsedag: {person.Birthday}\tMors-O-Meter: {person.CountDownTick}");
            }
        }
    }
}