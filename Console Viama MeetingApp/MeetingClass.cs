
namespace Visma_meeting_application
{
    internal class MeetingClass
    {
        public MeetingClass(int id, string meetingName, string responsiblePerson, string description,
            string category, string type, DateTime startDate, DateTime endDate, List<ParticipantClass> participant)
        {
            Id = id;
            MeetingName = meetingName;
            ResponsiblePerson = responsiblePerson;
            Description = description;
            Category = category;
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            Participant = participant;
        }
        public int Id { get; private set; }
        public string MeetingName { get; private set; }
        public string ResponsiblePerson { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public string Type { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<ParticipantClass> Participant { get; private set; }

        public class ParticipantClass
        {
            public ParticipantClass(string name, DateTime addDateTime)
            {
                Name = name;
                AddDateTime = addDateTime;
            }
            public string Name { get; set; }
            public DateTime AddDateTime { get; set; }
        }


        public void SetNewId(int id)
        {
            Id = id;
        }

        public void PrintPrarticipantList()
        {
            Console.WriteLine($"Meeting ID: {Id} \nMeeting name: {MeetingName} \nResponsible person: {ResponsiblePerson} \nDescription: {Description}" +
                $"\nCategory: {Category}\nType: {Type} \nStart date: {StartDate}\nEnd date: {EndDate}\nParticipants:{Participant.Count}");
            foreach (var participant in Participant)
            {
                Console.WriteLine(" Name: " + participant.Name + " Add time: " + participant.AddDateTime);
            }
        }

        public override string ToString()
        {
            PrintPrarticipantList();
            return "----------------------";
        }
    }
}
