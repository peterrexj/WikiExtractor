using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Models
{
    public class WikiDataCleanerModel
    {
        public bool Ignored { get; set; }
        public bool DuplicateName { get; set; } 
        public bool DuplicateLink { get; set; } 
        public bool DuplicateContent { get; set; }
        public bool RequireCheck => Ignored || DuplicateName || DuplicateLink || DuplicateContent;
        public PersonaViewModel Item { get; set; }
    }

    public class WikiDataCleanerWriteModel
    {
        public WikiDataCleanerWriteModel()
        {

        }

        public WikiDataCleanerWriteModel(WikiDataCleanerModel item)
        {
            Item = item;
        }
        public bool Ignored => Item.Ignored;
        public bool DuplicateName => Item.DuplicateName;
        public bool DuplicateLink => Item.DuplicateLink;
        public bool DuplicateContent => Item.DuplicateContent;
        public bool RequireCheck => Item.RequireCheck;
        public string Name => Item.Item.Name;
        public string WikiPath => $"https://en.wikipedia.org{Item.Item.WikiPath}";
        public int Id => Item.Item.Id;
        public string Tags => string.Join(",", Item.Item.Tags);
        private WikiDataCleanerModel Item { get; set; }
    }
}
