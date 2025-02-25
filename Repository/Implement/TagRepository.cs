using DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implement
{
    public class TagRepository : ITagRepository
    {
        private readonly FunewsManagementContext _context;

        public TagRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public Task<List<Tag>> GetAllTagsAsync()
        {
            return _context.Tags.Include(t => t.NewsArticles).ToListAsync();
        }
    }
}
