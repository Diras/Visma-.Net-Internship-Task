
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
                string inputCommandStr = Console.ReadLine();
                
                int inputCommand = GetIntFromString(inputCommandStr);

                switch (inputCommand)
                {
                    case 0:
                        {
                            Console.WriteLine("Enter meeting name");
                            string name = Console.ReadLine();
                            Console.WriteLine("Enter responsable person name");
                            string responsiblePerson = Console.ReadLine();
                            Console.WriteLine("Enter description");
                            string description = Console.ReadLine();
                            Console.WriteLine("Choose category: \n1 - CodeMonkey \n2 - Hub \n3 - Short \n4 - TeamBuilding");
                            string inputCategory = Console.ReadLine();
                            int categoryNumber = GetIntFromString(inputCategory);
                            string[] categories = { "CodeMonkey", "Hub", "Short", "TeamBuilding" };
                            string category = categories[categoryNumber - 1];
                            Console.WriteLine("Choose type: \n1 - Live \n2 - InPerson");
                            string inputType = Console.ReadLine();
                            int typeNumber = GetIntFromString(inputType);
                            string[] types = { "Live", "InPerson" };
                            string type = types[typeNumber - 1];
                            Console.WriteLine("Enter start date and time (e.g. 2022-07-22 10:00)");
                            DateTime startDateTime = DateTime.Parse(Console.ReadLine());
                            Console.WriteLine("Enter end date and time (e.g. 2022-07-22 10:00)");
                            DateTime endDateTime = DateTime.Parse(Console.ReadLine());

                            List<MeetingClass.ParticipantClass> participant = new List<MeetingClass.ParticipantClass>();
                            MeetingClass newMeeting = new MeetingClass(0, name, responsiblePerson, description, category, type, startDateTime, endDateTime, participant);
                            newMeeting.Participant.Add(new MeetingClass.ParticipantClass(responsiblePerson, DateTime.Now));
                            SaveToFile(newMeeting);

                            break;
                        }
                    case 1:
                        {

                            Console.WriteLine("Enter meeting ID:");
                            string idStr = Console.ReadLine();
                            int id = GetIntFromString(idStr);
                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();

                            if (allCurrentMeetings.Exists(x => x.Id == id))
                            {
                                string responsiblePerson = allCurrentMeetings[id - 1].ResponsiblePerson.ToString();
                                Console.WriteLine("Input your name:");
                                string name = Console.ReadLine();
                                if (name == responsiblePerson)
                                {
                                    DeleteFromFile(id);
                                    Console.WriteLine("Succsess!");
                                    Console.WriteLine("-------------------");
                                }
                                else Console.WriteLine("Only responsible person can delete!");
                            }
                            else Console.WriteLine("Id not found");
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("Enter meeting ID:");
                            string idStr = Console.ReadLine();
                            bool addingParticipant = true;
                            while (addingParticipant)
                            {
                                Console.WriteLine("\n0 - Add new participant \n1 - Remove participant \n2 - Exit");
                                inputCommand = GetIntFromString(Console.ReadLine());
                                switch (inputCommand)
                                {
                                    case 0:
                                        {
                                            Console.WriteLine("Enter participant name:");
                                            string participantName = Console.ReadLine();
                                            int id = GetIntFromString(idStr);
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            if (allCurrentMeetings[id - 1].Participant.Exists(x => x.Name == participantName))
                                            {
                                                Console.WriteLine("Person already at the meeting!");
                                            }
                                            else
                                            {
                                                allCurrentMeetings[id - 1].Participant.Add(new MeetingClass.ParticipantClass(participantName, DateTime.Now));
                                                SaveToFile(allCurrentMeetings);
                                                Console.WriteLine("Succsess!");
                                                Console.WriteLine("-------------------");
                                            }
                                            break;
                                        }
                                    case 1:
                                        {
                                            Console.WriteLine("Enter participant name:");
                                            string participantName = Console.ReadLine();
                                            int id = GetIntFromString(idStr);
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();

                                            if (allCurrentMeetings[id - 1].Participant.Exists(x => x.Name == participantName))
                                            {
                                                var itemToDelete = allCurrentMeetings[id - 1].Participant.Single(r => r.Name == participantName);
                                                if (itemToDelete != null && participantName != allCurrentMeetings[id - 1].ResponsiblePerson)
                                                {
                                                    allCurrentMeetings[id - 1].Participant.Remove(itemToDelete);
                                                    SaveToFile(allCurrentMeetings);
                                                    Console.WriteLine("Succsess!");
                                                    Console.WriteLine("-------------------");
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
                                inputCommand = GetIntFromString(Console.ReadLine());
                                switch (inputCommand)
                                {
                                    case 0:
                                        {
                                            Console.WriteLine("Filter by description\n Srearch for...");
                                            string searchName = Console.ReadLine();
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            var meetingsFilltered = allCurrentMeetings.FindAll(r => r.Description.Equals(searchName));
                                            foreach (var meeting in meetingsFilltered) Console.WriteLine(meeting);
                                            break;
                                        }
                                    case 1:
                                        {
                                            Console.WriteLine("Filter by responsible person\n Srearch for...");
                                            string searchName = Console.ReadLine();
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            var meetingsFiltered = allCurrentMeetings.FindAll(r => r.ResponsiblePerson == searchName);
                                            foreach (var meeting in meetingsFiltered) Console.WriteLine(meeting);
                                            break;
                                        }
                                    case 2:
                                        {
                                            Console.WriteLine("Filter by category\n Srearch for...");
                                            string searchName = Console.ReadLine();
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            var meetingsFiltered = allCurrentMeetings.FindAll(r => r.Category == searchName);
                                            foreach (var meeting in meetingsFiltered) Console.WriteLine(meeting);
                                            break;
                                        }
                                    case 3:
                                        {
                                            Console.WriteLine("Filter by type\n Srearch for...");
                                            string searchName = Console.ReadLine();
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            var meetingsFiltered = allCurrentMeetings.FindAll(r => r.Type == searchName);
                                            foreach (var meeting in meetingsFiltered) Console.WriteLine(meeting);
                                            break;
                                        }
                                    case 4:
                                        {
                                            Console.WriteLine("Filter by date\n Srearch from...(eg. 2022-01-01)");
                                            DateTime searchName = DateTime.Parse(Console.ReadLine());
                                            Console.WriteLine("Srearch to...(eg.2022-02-01)");
                                            DateTime searchName2 = DateTime.Parse(Console.ReadLine());

                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            var meetingsFiltered = allCurrentMeetings.FindAll(r => r.StartDate >= searchName && r.EndDate <= searchName2);
                                            foreach (var meeting in meetingsFiltered) Console.WriteLine(meeting);
                                            break;
                                        }
                                    case 5:
                                        {
                                            Console.WriteLine("Filter by number of attendees\n Srearch for...");
                                            int searchName = int.Parse(Console.ReadLine());
                                            List<MeetingClass> allCurrentMeetings = ReadAllFromFile();
                                            var meetingsFiltered = allCurrentMeetings.FindAll(r => r.Participant.Count == searchName);
                                            foreach (var meeting in meetingsFiltered) Console.WriteLine(meeting);
                                            break;
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

        static int GetIntFromString(string inputStr)
        {
            int input = 0;
            try
            {
                input = int.Parse(inputStr);
            }
            catch (FormatException)
            {
                Console.WriteLine("Wrong command");
            }
            return input;
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


