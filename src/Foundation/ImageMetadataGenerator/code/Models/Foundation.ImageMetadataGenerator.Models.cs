// ReSharper Disable all
using global::Sitecore.Data;
using global::Sitecore.Data.Fields;
using global::Sitecore.Data.Items;
using global::System.CodeDom.Compiler;

namespace Foundation.ImageMetadataGenerator.Models
{
	[GeneratedCode("Leprechaun", "2.2.4.0")]
	public interface IChatGptVisionApiSettingsItem
	{
		TextField ApiKeyField { get; }
		ReferenceField DetailField { get; }
		TextField MaxTokensField { get; }
		TextField ModelField { get; }
		TextField UrlField { get; }
	}
	[GeneratedCode("Leprechaun", "2.2.4.0")]
	public class ChatGptVisionApiSettings : CustomItem, IChatGptVisionApiSettingsItem
	{
		public ChatGptVisionApiSettings(Item innerItem)
			:base(innerItem)
		{
		}
		public static string TemplateName => "ChatGPT Vision API Settings";
		public static ID ItemTemplateId => new ID("{ACB5AC0F-FDC8-43F2-840F-69F0B069E32A}");
		
		public TextField ApiKeyField => new TextField(InnerItem.Fields[FieldConstants.ApiKey.Id]);
		public ReferenceField DetailField => new ReferenceField(InnerItem.Fields[FieldConstants.Detail.Id]);
		public TextField MaxTokensField => new TextField(InnerItem.Fields[FieldConstants.MaxTokens.Id]);
		public TextField ModelField => new TextField(InnerItem.Fields[FieldConstants.Model.Id]);
		public TextField UrlField => new TextField(InnerItem.Fields[FieldConstants.Url.Id]);
		public static implicit operator ChatGptVisionApiSettings(Item item) => item != null ? new ChatGptVisionApiSettings(item) : null;
		public static implicit operator Item(ChatGptVisionApiSettings customItem) => customItem?.InnerItem;
		public struct FieldConstants
		{
			public struct ApiKey
            {
		        public const string FieldName = "API Key";
		        public static readonly ID Id = new ID("{DEBB95EC-D1E1-456A-9DA2-2A502D32C8A0}");
            }
            public struct Detail
            {
		        public const string FieldName = "Detail";
		        public static readonly ID Id = new ID("{F36E7244-909D-4D33-B59E-FD2BA9B485E3}");
            }
            public struct MaxTokens
            {
		        public const string FieldName = "Max Tokens";
		        public static readonly ID Id = new ID("{DBE3617F-095B-401A-ACB4-FA724DF7B019}");
            }
            public struct Model
            {
		        public const string FieldName = "Model";
		        public static readonly ID Id = new ID("{36C7EF02-4A04-4DA6-B15A-DB2471C363C4}");
            }
            public struct Url
            {
		        public const string FieldName = "Url";
		        public static readonly ID Id = new ID("{66EB94FE-E0A1-40E9-BA60-C7423CD1A256}");
            }
            
		}
	}
}


