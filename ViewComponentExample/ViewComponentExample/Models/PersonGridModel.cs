namespace ViewComponentExample.Models
{
    public class PersonGridModel
    {
        public string GridTitle { get; set; }=string.Empty;
        public List<Person> Person { get; set; } = new List<Person>(); 
    }
}
