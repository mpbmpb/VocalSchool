namespace VocalSchool.Models
{
    public class Change
    {
        public int ChangeId { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string PropertyName { get; set; }
        public string Value { get; set; }
        public int? ForeignKey { get; set; }
        public int CourseId { get; set; }    
        public virtual Course Course { get; set; }
    }
}