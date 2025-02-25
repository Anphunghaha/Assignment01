using DataAccess.Model;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class TagService : ITagService
    {
        private ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public Task<List<Tag>> GetAllTagsAsync()
        {
            return _tagRepository.GetAllTagsAsync();
        }
    }
}
