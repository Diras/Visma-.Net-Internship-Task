
using Newtonsoft.Json;


namespace Visma_meeting_application
{
    internal class Program
    {
        static string FilePath { get; set; }
        static void Main(string[] args)
        {
            string fileName = "MeetingForVisma.txt";
            string FileLocation = Path.GetTempPath();
            FilePath = FileLocation + fileName;

            if (File.Exists(FilePath) == false)
            {
                var file = File.Create(FilePath);
                file.Close();
            }

            string allCommands = "\n0 - Add new meeting \n1 - Delete meeting \n2 - Edit participants" +
                "\n3 - Show all meetings \n4 - Filter meetings\n5 - Close application   \n-------------------";
            
            bool isWork = true;

            while (isWork)
            {
                Console.WriteLine(allCommands);
               
                switch (ValidateInt())
                {
                    case 0:
                        {
                            Console.WriteLine("Enter meeting name");
                            string name = ValidateName();

                            Console.WriteLine("Enter responsable person name");
                            string responsiblePerson = ValidateName();

                            Console.WriteLine("Enter description");
                            string description = Console.ReadLine();

                            Console.WriteLine("Choose category: \n1 - CodeMonkey \n2 - Hub \n3 - Short \n4 - TeamBuilding");
                            int categoryNumber = ValidateIntRange(1,4);
                            string[] categories = { "CodeMonkey", "Hub", "Short", "TeamBuilding" };
                            string category = categories[categoryNumber - 1];

                            Console.WriteLine("Choose type: \n1 - Live \n2 - InPerson");
                            int typeNumber = ValidateIntRange(1,2);
                            string[] types = { "Live", "InPerson" };
                            string type = types[typeNumber - 1];

                            DateTime startDateTime = DateTime.Now;
                            DateTime endDateTime = DateTime.Now;

                            bool checkDate = true;
                            while (checkDate)
                            {
                                Console.WriteLine("Enter start date and time");
                                startDateTime = DateTimeValidation();
                                Console.WriteLine("Enter end date and time");
                                endDateTime = DateTimeValidation();
                                if (startDateTime > endDateTime) Console.WriteLine("\nStart date can't be later than end date!");
                                else if( startDateTime == endDateTime) Console.WriteLine("\nStart date can't be the same as end date!");
                                else checkDate = false;
                            }
                            List<MeetingClass.ParticipantClass> participant = new List<MeetingClass.ParticipantClass>();
                            MeetingClass newMeeting = new MeetingClass(0, name, responsiblePerson, description, category, type, startDateTime, endDateTime, participant);
                            newMeeting.Participant.Add(new MeetingClass.ParticipantClass(responsiblePerson, DateTime.Now));
                            SaveToFile(newMeeting);
                            Succsess();
                            break;
                        }
                    case 1:
                        {
                            Console.WriteLine("Enter meeting ID:");
                            
                            int id = ValidateInt();
                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();

                            if (allCurrentMeetings.Exists(x => x.Id == id))
                            {
                                string responsiblePerson = allCurrentMeetings[id - 1].ResponsiblePerson.ToString();
                                Console.WriteLine("Input your name:");
                                string name = Console.ReadLine();
                                if (name.ToLower() == responsiblePerson.ToLower())
                                {
                                    DeleteFromFile(id);
                                    Succsess();
                                }
                                else Console.WriteLine("Only responsible person can delete!");
                            }
                            else Console.WriteLine("Id not found");
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("Enter meeting ID:");
                            int id = ValidateInt();
                            bool addingParticipant = true;
                            while (addingParticipant)
                            {
                                Console.WriteLine("\n0 - Add new participant \n1 - Remove participant \n2 - Exit");
                                
                                switch (ValidateInt())
                                {
                                    case 0:
                                        {
                                            Console.WriteLine("Enter participant name:");
                                            string participantName = Console.ReadLine();
                                            
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            if (allCurrentMeetings[id - 1].Participant.Exists(x => x.Name.ToLower() == participantName.ToLower()))
                                            {
                                                Console.WriteLine("Person already at the meeting!");
                                            }
                                            else
                                            {
                                                allCurrentMeetings[id - 1].Participant.Add(new MeetingClass.ParticipantClass(participantName, DateTime.Now));
                                                SaveToFile(allCurrentMeetings);
                                                Succsess();
                                            }
                                            break;
                                        }
                                    case 1:
                                        {
                                            Console.WriteLine("Enter participant name:");
                                            string participantName = Console.ReadLine();
                                            
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();

                                            if (allCurrentMeetings[id - 1].Participant.Exists(x => x.Name.ToLower() == participantName.ToLower()))
                                            {
                                                var itemToDelete = allCurrentMeetings[id - 1].Participant.Single(r => r.Name == participantName);
                                                if (itemToDelete != null && participantName.ToLower() != allCurrentMeetings[id - 1].ResponsiblePerson.ToLower())
                                                {
                                                    allCurrentMeetings[id - 1].Participant.Remove(itemToDelete);
                                                    SaveToFile(allCurrentMeetings);
                                                    Succsess();
                                                }
                                                else Console.WriteLine("Reponsible person can't be removed!");
                                            }
                                            else Console.WriteLine("Person not found!");
                                            break;
                                        }
                                    case 2:
                                        {
                                            addingParticipant = false;
                                            break;
                                        }
                                    default:
                                        {
                                            Console.WriteLine("Action denied, command error");
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    case 3:
                        {
                            var allMeetings = ReadAllFromFile();
                            if (allMeetings.Count == 0) Console.WriteLine("0 Visma meetings");
                            foreach (var meeting in allMeetings) Console.WriteLine(meeting);
                            break;
                            
                        }
                    case 4:
                        {
                            bool filtering = true;
                            while (filtering)
                            {
                                Console.WriteLine("\n0 - Filter by description \n1 - Filter by responsalbe person \n2 - Filter by category" +
                                "\n3 - Filter by type\n4 - Filter by dates\n5 - Filter by number of attendees\n6 - Exit");
                                
                                switch (ValidateInt())
                                {
                                    case 0:
                                        {
                                            Console.WriteLine("Filter by description\n Srearch for...");
                                            string searchName = Console.ReadLine();
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            var meetingsFiltered = allCurrentMeetings.FindAll(r => r.Description.ToLower().Contains(searchName.ToLower()));
                                            if (meetingsFiltered.Count == 0) Console.WriteLine("\n0 meeting found");
                                            else foreach (var meeting in meetingsFiltered) Console.WriteLine(meeting);
                                            break;
                                        }
                                    case 1:
                                        {
                                            Console.WriteLine("Filter by responsible person\n Srearch for...");
                                            string searchName = Console.ReadLine();
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            var meetingsFiltered = allCurrentMeetings.FindAll(r => r.ResponsiblePerson.ToLower() == searchName.ToLower());
                                            if (meetingsFiltered.Count == 0) Console.WriteLine("\n0 meeting found");
                                            else foreach (var meeting in meetingsFiltered) Console.WriteLine(meeting);
                                            break;
                                        }
                                    case 2:
                                        {
                                            Console.WriteLine("Filter by category\nSelect:\n1 - CodeMonkey \n2 - Hub \n3 - Short \n4 - TeamBuilding");
                                            int result = ValidateInt();
                                            string[] categories = { "CodeMonkey", "Hub", "Short", "TeamBuilding" };
                                            string category = categories[result - 1];
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            var meetingsFiltered = allCurrentMeetings.FindAll(r => r.Category == category);
                                            if (meetingsFiltered.Count == 0) Console.WriteLine("\n0 meeting found");
                                            else foreach (var meeting in meetingsFiltered) Console.WriteLine(meeting);
                                            break;
                                        }
                                    case 3:
                                        {
                                            Console.WriteLine("Filter by type\nSelect:\n1 - Live \n2 - InPerson");
                                            int result = ValidateInt();
                                            string[] types = { "Live", "InPerson" };
                                            string type = types[result - 1];
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            var meetingsFiltered = allCurrentMeetings.FindAll(r => r.Type == type);
                                            if (meetingsFiltered.Count == 0) Console.WriteLine("\n0 meeting found");
                                            else foreach (var meeting in meetingsFiltered) Console.WriteLine(meeting);
                                            break;
                                        }
                                    case 4:
                                        {
                                            Console.WriteLine("Filter by date\n Srearch from...(eg. 2022-01-01)");
                                            DateTime searchFromDate = DateValidation();
                                            Console.WriteLine("Srearch to...(eg.2022-02-01)");
                                            DateTime searchToDate = DateValidation();

                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            var meetingsFiltered = allCurrentMeetings.FindAll(r => r.StartDate >= searchFromDate && r.EndDate <= searchToDate);
                                            if (meetingsFiltered.Count == 0) Console.WriteLine("\n0 meeting found");
                                            else foreach (var meeting in meetingsFiltered) Console.WriteLine(meeting);
                                            break;
                                        }
                                    case 5:
                                        {
                                            Console.WriteLine("Filter by number of attendees\n Srearch for meetings that have over ... people attending");
                                            int result = ValidateInt();
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            var meetingsFiltered = allCurrentMeetings.FindAll(r => r.Participant.Count >= result);
                                            if (meetingsFiltered.Count == 0) Console.WriteLine("\n0 meeting found");
                                            else foreach (var meeting in meetingsFiltered) Console.WriteLine(meeting);
                                            break;
                                        }
                                    case 6:
                                        {
                                            filtering = false;
                                            break;
                                        }
                                    default:
                                        {
                                            Console.WriteLine("Action denied, command error");
                                            break;
                                        }
                                }
                            }
                            break;
                            
                        }
                    case 5:
                        {
                            isWork = false;
                            Console.WriteLine("Closing application");
                            break;
                        }

                    default:
                        {
                            Console.WriteLine("Action denied, command error");
                            break;
                        }
                }
            }
        }

        static void Succsess()
        {
            Console.WriteLine("\nSuccsess!\n------------------");
        }
        static int ValidateInt()
        {
            
            int result = 0;
            bool testing = true;
            while (testing)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out result)) testing = false;
                else Console.WriteLine("Not a number");
            }
            return result;
        }
        static string ValidateName()
        {
            string input= "";
            bool testing = true;
            while (testing)
            {
                input = Console.ReadLine();
                if (input.Length < 2) Console.WriteLine("Name is too short");
                else if (input.Length > 50) Console.WriteLine("Name is too long");
                else testing = false;

            }
            return input;
        }
        static int ValidateIntRange(int from, int to)
        {
            int result = 0;
            bool testing = true;
            while (testing)
            {

                result = ValidateInt();
                if (result >= from && result <= to )
                {
                    testing = false;
                }
                else Console.WriteLine("Number out of range!");

            }
            return result;
        }
        static DateTime DateTimeValidation()
        {
            DateTime result = DateTime.Now;
            bool testing = true;
            while (testing)
            {
                Console.WriteLine("Enter date: (e.g. 2022-07-26)");
                string inputDate = Console.ReadLine();
                Console.WriteLine("Enter time: (e.g. 10:00)");
                string inputTime = Console.ReadLine();

                
                if (DateTime.TryParse(inputDate +" "+ inputTime, out result)) 
                {
                    testing = false;
                }
                else Console.WriteLine("Input is incorrect!");

            }
            return result;
        }
        static DateTime DateValidation()
        {
            DateTime result = DateTime.Now;
            bool testing = true;
            while (testing)
            {
                Console.WriteLine("Enter date: (e.g. 2022-07-26)");
                string inputDate = Console.ReadLine();
                


                if (DateTime.TryParse(inputDate, out result))
                {
                    testing = false;
                }
                else Console.WriteLine("Input is incorrect!");

            }
            return result;
        }

        static void SaveToFile(MeetingClass meeting)
        {
            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();

            int lastId = allCurrentMeetings.Count == 0 ? 0 : allCurrentMeetings.Last().Id;

            meeting.SetNewId(lastId + 1);

            allCurrentMeetings.Add(meeting);

            string serealizedMeetings = JsonConvert.SerializeObject(allCurrentMeetings);

            File.WriteAllText(FilePath, serealizedMeetings);
        }
        static void SaveToFile(List<MeetingClass> meetings)
        {

            string serealizedMeetings = JsonConvert.SerializeObject(meetings);

            File.WriteAllText(FilePath, serealizedMeetings);
        }
        static bool DeleteFromFile(int id)
        {
            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
            MeetingClass meetingForDeletion = allCurrentMeetings.FirstOrDefault(me => me.Id == id);

            bool result = false;

            if (meetingForDeletion != null)
            {
                allCurrentMeetings.Remove(meetingForDeletion);
                SaveToFile(allCurrentMeetings);
                result = true;
            }
            return result;
        }
        static List<MeetingClass> ReadAllFromFile()
        {
            string json = File.ReadAllText(FilePath);

            List<MeetingClass> currentMeetings = JsonConvert.DeserializeObject<List<MeetingClass>>(json);

            return currentMeetings ?? new List<MeetingClass>();
        }
    }
}


