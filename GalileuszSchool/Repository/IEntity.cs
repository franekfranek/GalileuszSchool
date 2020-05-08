namespace GalileuszSchool.Repository
{
    public interface IEntity
    {
        public int Id { get; set; }

        public string Slug { get; set; }
    }
}