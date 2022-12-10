using Pj.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WikiExtractor.DbModels;
using WikiExtractor.Models;

namespace WikiExtractor.Exts
{
    public static class ModelExtensions
    {
        public static string Content(this List<WikiParagraphDetailModel> models)
        {
            if (models == null || models.IsEmpty()) return string.Empty;
            return string.Join(Environment.NewLine, models.Select(s => s.ContentBuilder));
        }

        public static WikiPicture ToImageDbModel(this MetaDataModel model)
        {
            var image = new WikiPicture();
            image.Path = GetValueMetaDataAttribute(model.CustomMetadata, "src");
            if (image.Path.StartsWith("//"))
            {
                //image.Path = $"https:{HttpUtility.UrlDecode(image.Path)}";
                image.Path = $"https:{image.Path}";
            }
            image.Width = GetValueMetaDataAttribute(model.CustomMetadata, "data-file-width").ToInteger();
            image.Height = GetValueMetaDataAttribute(model.CustomMetadata, "data-file-height").ToInteger();
            if (model.Description.HasValue())
            {
                image.Caption = HtmlAgilityEx.DecodedInnerText(model.Description!, removeNewLine: true);
            }
            else
            {
                image.Caption = HtmlAgilityEx.DecodedInnerText(GetValueMetaDataAttribute(model.CustomMetadata, "alt"), removeNewLine: true);
            }
            if (image.Caption.HasValue() && image.Caption!.Contains("."))
            {
                image.Caption = Path.GetFileNameWithoutExtension(image.Caption);
            }
            image.Sequence = model.Sequence;
            return image;
        }

        public static WikiPicture ToImageDbModel(this WikiPictureModel model)
        {
            var image = new WikiPicture();
            image.Path = GetValueMetaDataAttribute(model.CustomMetadata, "src");
            if (image.Path.StartsWith("//"))
            {
                image.Path = $"https:{HttpUtility.UrlDecode(image.Path)}";
            }
            image.Width = GetValueMetaDataAttribute(model.CustomMetadata, "data-file-width").ToInteger();
            image.Height = GetValueMetaDataAttribute(model.CustomMetadata, "data-file-height").ToInteger();
            if (model.Caption.HasValue())
            {
                image.Caption = HtmlAgilityEx.DecodedInnerText(model.Caption!, removeNewLine: true);
            }
            else
            {
                image.Caption = HtmlAgilityEx.DecodedInnerText(GetValueMetaDataAttribute(model.CustomMetadata, "alt"), removeNewLine: true);
            }
            if (image.Caption.HasValue() &&
                image.Caption!.Contains(".") &&
                image.Caption!.SplitAndTrim(".").Last()?.Length < 4)
            {
                image.Caption = Path.GetFileNameWithoutExtension(image.Caption);
            }
            image.Sequence = model.Sequence;
            return image;
        }

        private static string GetValueMetaDataAttribute(Dictionary<string, string> customMetadata, string key)
        {
            return customMetadata.ContainsKey(key) && customMetadata[key].HasValue() ? customMetadata[key] : string.Empty;
        }

        public static List<WikiWhatToExtractModel> WithDefaultFilters(this List<WikiWhatToExtractModel> data)
        {
            return data.Where(f => !f.Title.ContainsIgnoreCase("page does not exist")).ToList();
        }
    }
}
