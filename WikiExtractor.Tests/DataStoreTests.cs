using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process;
using WikiExtractor.Repository;

namespace WikiExtractor.Tests
{
    internal class DataStoreTests
    {
        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Have_Menus(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            Assert.IsTrue(menuItems.Count() > 0);
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Have_Tag_OnMenu(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            var grp = menuItems.Where(f => f.Tags.IsEmpty());
            Assert.IsTrue(grp.Count() == 0);
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Have_Tag_Data_EachMenu(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            foreach (var menuItem in menuItems)
            {
                var data = wikiAppController.GetListOfWikiItems(new List<string> { menuItem.Tags });
                Assert.IsTrue(data.Any());
            }
            //Assert.IsTrue(grp.Count() == 0);
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Not_Have_Duplicate_MenuItemName(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            var grp = menuItems.GroupBy(f => f.MenuItemName).Select(f => new { f.Key, Childs = f.ToList() })
                .Where(f => f.Childs.Count > 1)
                .ToList();
            Assert.IsTrue(grp.Count() == 0);
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Not_Have_Duplicate_Menu_TitleOnThePage(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            var grp = menuItems.GroupBy(f => f.TitleOnThePage).Select(f => new { f.Key, Childs = f.ToList() })
                .Where(f => f.Childs.Count > 1)
                .ToList();
            Assert.IsTrue(grp.Count() == 0);
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Not_Have_Duplicate_Menu_Tag(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            var grp = menuItems.GroupBy(f => f.Tags).Select(f => new { f.Key, Childs = f.ToList() })
                .Where(f => f.Childs.Count > 1)
                .ToList();
            Assert.IsTrue(grp.Count() == 0);
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Have_Data_EachItem(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            foreach (var menuItem in menuItems)
            {
                var data = wikiAppController.GetListOfWikiItems(new List<string> { menuItem.Tags });
                foreach (var item in data)
                {

                    var eachItemDetail = wikiAppController.GetViewModelById(item.Id);
                    //if (ignoreRouteList.Contains(eachItemDetail.WikiPath)) continue;
                    Assert.IsNotNull(eachItemDetail);
                    Assert.IsTrue(eachItemDetail.Name.HasValue());
                    Assert.IsTrue(eachItemDetail.WikiPath.HasValue());
                    Assert.IsFalse(eachItemDetail.Pictures.IsEmpty() &&
                        eachItemDetail.Metadatas.IsEmpty() &&
                        eachItemDetail.Pictures.IsEmpty());
                }
            }
        }

        public static IEnumerable<string> DatabaseFiles
        {
            get
            {
                return Directory.EnumerateFiles(
                    IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder,
                    "WikiExtractor\\bin\\Debug\\net6.0\\Db"), "*.db");
            }
        }
    }
}
