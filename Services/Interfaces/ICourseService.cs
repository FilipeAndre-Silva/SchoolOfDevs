using SchoolOfDevs.Entities;

namespace SchoolOfDevs.Services.Interfaces
{
    public interface ICourseService
    {
        public Task<Course> Create(Course course);
        public Task<Course> GetById(int id);
        public Task<List<Course>> GetAll();
        public Task Update(Course courseIn, int id);
        public Task Delete(int id);
    }
}