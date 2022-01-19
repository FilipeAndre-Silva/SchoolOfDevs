using SchoolOfDevs.Entities;

namespace SchoolOfDevs.Services.Interfaces
{
    public interface INoteService
    {
        public Task<Note> Create(Note note);
        public Task<Note> GetById(int id);
        public Task<List<Note>> GetAll();
        public Task Update(Note noteIn, int id);
        public Task Delete(int id);
    }
}