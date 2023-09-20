using System.ComponentModel.DataAnnotations;

namespace NZEJobServices.Types;

/// <summary>
/// Represents the job structure in the entity framework context. The entity framework will handle mapping these fields
/// to relevant database tables or structures.
/// </summary>
public class Job
{
    /// <summary>
    /// Unique job identifier
    /// </summary>
    public Guid ID { get; set; }

    /// <summary>
    /// Job status priority
    /// </summary>
    public JobUrgency Urgency { get; set; }

    /// <summary>
    /// Name of job
    /// </summary>
    [Required] // data annotation of required will specify in the output schema tha a field cannot be null
    public string Title { get; set; }

    /// <summary>
    /// Job general description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Who job is for
    /// </summary>
    [Required] // data annotation of required will specify in the output schema tha a field cannot be null
    public string ClientName { get; set; }

    /// <summary>
    /// who is responsible for the job
    /// </summary>
    [Required]
    public string StaffName { get; set; }

    /// <summary>
    /// client contact number
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// location of job
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Current job stage
    /// </summary>
    public JobStage Stage { get; set; }

    /// <summary>
    /// scheduled installation date
    /// </summary>
    public DateTime InstallationDate { get; set; }

    /// <summary>
    /// default constructor
    /// </summary>
    /// <param name="urgency"></param>
    /// <param name="title"></param>
    /// <param name="description"></param>
    /// <param name="clientName"></param>
    /// <param name="staffName"></param>
    /// <param name="phone"></param>
    /// <param name="address"></param>
    /// <param name="installationDate"></param>
    public Job(JobUrgency urgency, string title, string description, string clientName, string staffName, string phone, string address, DateTime installationDate)
    {
        ID = Guid.NewGuid();
        Urgency = urgency;
        Title = title;
        Description = description;
        ClientName = clientName;
        StaffName = staffName;
        Phone = phone;
        Address = address;
        InstallationDate = installationDate;
        Stage = JobStage.NotStarted;
    }

    #region Example Generation
    /// <summary>
    /// creates a new randomised sample job
    /// </summary>
    /// <returns></returns>
    public static Job NewSample()
    {
        var rand = new Random();
        var urgency = Enum.GetValues<JobUrgency>();
        var stage = Enum.GetValues<JobStage>();
        var titles = new string[] {
                "Solar roof installation",
                "Energy efficiency lighting upgrade",
                "Solar hot water system",
                "Energy efficiency utility replacement",
                "Passive cooling system conversion",
                "Resistive heating system replacement"
            };
        var descriptions = new string[] {
                "Job detailed description goes here"
            };
        var names = new string[] {
                "Joe Blogs",
                "Jane Doe",
                "Joel Kajetan",
                "Séraphine Priyanka",
                "Joel Shawnee",
                "Miluše Asma",
                "Julius Sigrid",
                "Neilos Inés",
                "Domagoj Gerlinde",
                "Evandrus Gabino",
                "Evgenia Sheridan",
                "Ljilja Sedna",
            };
        var phone = "0400 000 000";
        var addresses = new string[] {
                "7671 Anna Meadow, Suite 366, 7757, Port Mackenziechester, Western Australia, Australia",
                "72 Towne Knoll, Apt. 947, 3311, Blakeville, Victoria, Australia",
                "98 Jonathan Pass, Suite 926, 6309, Hayesside, New South Wales, Australia",
                "7294 Kuhn Road, Suite 691, 3132, West Alexandra, New South Wales, Australia",
                "43 Lincoln Circle, Suite 913, 8610, Samuelburgh, Tasmania, Australia",
                "490 Lincoln Ridge, Apt. 597, 5074, New Samanthaport, Western Australia, Australia",
                "1254 Greenfelder Island, Apt. 323, 2869, East Maddisonmouth, New South Wales, Australia",
                "5393 Heidi Summit, Suite 500, 8248, Lake Mitchellburgh, Tasmania, Australia",
                "53 Alexis Terrace, Apt. 440, 6120, Sophiashire, New South Wales, Australia",
                "097 Doherty Boulevard, Suite 951, 3073, Mayamouth, South Australia, Australia",
            };
        var day = DateTime.Now;
        day.AddDays(rand.NextDouble() * 100);

        var result = new Job(
            (JobUrgency)urgency[rand.Next(urgency.Length)],
            titles[rand.Next(titles.Length)],
            descriptions[rand.Next(descriptions.Length)],
            names[rand.Next(names.Length)],
            names[rand.Next(names.Length)],
            phone,
            addresses[rand.Next(addresses.Length)],
            day);

        result.Stage = (JobStage)stage[rand.Next(stage.Length)];

        return result;
    }
    #endregion
}

/// <summary>
/// Job urgency states
/// </summary>
public enum JobUrgency
{
    /// <summary>
    /// Low
    /// </summary>
    Low,
    /// <summary>
    /// Medium
    /// </summary>
    Medium,
    /// <summary>
    /// High
    /// </summary>
    High,
    /// <summary>
    /// Critical
    /// </summary>
    Critical,
}

/// <summary>
/// Job stages
/// </summary>
public enum JobStage
{
    /// <summary>
    /// Not Started
    /// </summary>
    NotStarted,
    /// <summary>
    /// In Progress
    /// </summary>
    InProgress,
    /// <summary>
    /// Currently being audited
    /// </summary>
    InAudit,
    /// <summary>
    /// Completed
    /// </summary>
    Complete
}