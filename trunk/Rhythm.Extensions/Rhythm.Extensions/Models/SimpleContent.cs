﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Rhythm.Extensions.Models {
	/// <summary>
	/// Class that represents the bare minimum data for a page to serialize to JSON
	/// </summary>
	public class SimpleContent {
		public int Id;
		public string Name;
		public string Url;
		public int Level;
		public IEnumerable<SimpleContent> Children;
		public IEnumerable<int> ChildrenIds;
		public SimpleContentType ContentType;
		public SimpleTemplate Template;
		public Dictionary<string, object> Properties;

		/// <summary>
		/// Converts a SimpleContent instance to a JSON string.
		/// </summary>
		/// <returns>JSON string representation of the SimpleContent instance.</returns>
		public string ToJson() {
			return this.ToJObject().ToString();
		}

		/// <summary>
		/// Converts a SimpleContent instance to a JObject.
		/// </summary>
		/// <returns>JObject representation of the SimpleContent instance.</returns>
		public JObject ToJObject() {
			return JObject.FromObject(this);
		}

		/// <summary>
		/// Converts an IPublishedContent instance to SimpleContent.
		/// </summary>
		/// <param name="content">The IPublishedContent instance you wish to convert.</param>
		/// <param name="recurseChildren">Whether to include the children in the SimpleContent instance.</param>
		/// <param name="recurseContentTypes">Whether to include the parent content types on each SimpleContent instance.</param>
		/// <param name="recurseTemplates">Whether to include the parent templates on each SimpleContent instance.</param>
		/// <returns>A SimpleContent representation of the specified IPublishedContent</returns>
		public static SimpleContent FromIPublishedContent(IPublishedContent content, bool recurseChildren = true, bool recurseContentTypes = true, bool recurseTemplates = true) {
			if (content == null) return null;

			/*
			 * Using string, object for key/value pairs.
			 * An object is used so that the JavaScriptSerializer will
			 * automatically detect the type and serialize it to the
			 * correct JavaScript type.
			 */
			var properties =
				content.Properties
				       .Where(p => !String.IsNullOrWhiteSpace(p.Value.ToString()))
				       .ToDictionary(prop => prop.Alias, prop => prop.Value);
			
			var result = new SimpleContent() {
				Id = content.Id,
				Name = content.Name,
				Url = content.Url,
				Level = content.Level,
				Properties = properties,
				ContentType = SimpleContentType.FromContentType(content.DocumentTypeId, recurseContentTypes),
				Template = SimpleTemplate.FromTemplate(content.TemplateId, recurseTemplates),
				ChildrenIds = content.Children.Select(x => x.Id).ToList()
			};

			if (recurseChildren) {
				result.Children = FromIPublishedContent(content.Children, true, recurseContentTypes, recurseTemplates);
			}

			return result;
		}

		/// <summary>
		/// Converts a collection of IPublishedContent instances to a collection of SimpleContent instances.
		/// </summary>
		/// <param name="content">The IPublishedContent collection you wish to convert.</param>
		/// <param name="recurseChildren">Whether to include the children in the SimpleContent instance.</param>
		/// <param name="recurseContentTypes">Whether to include the parent content types on each SimpleContent instance.</param>
		/// <param name="recurseTemplates">Whether to include the parent templates on each SimpleContent instance.</param>
		/// <returns>A collection of SimpleContent instances</returns>
		public static IEnumerable<SimpleContent> FromIPublishedContent(IEnumerable<IPublishedContent> content, bool recurseChildren = true, bool recurseContentTypes = true, bool recurseTemplates = true) {
			return content.Select(x => FromIPublishedContent(x, recurseChildren, recurseContentTypes, recurseTemplates));
		}
	}

	/// <summary>
	/// Extension methods for the SimpleContent class and related classes.
	/// </summary>
	public static class SimpleContentExtensionMethods {
		/// <summary>
		/// Converts a collection of SimpleContent instances to a JSON array string.
		/// </summary>
		/// <param name="content">The SimpleContent collection you wish to convert.</param>
		/// <returns>JSON array string representation of the specified SimpleContent collection.</returns>
		public static string ToJson(this IEnumerable<SimpleContent> content) {
			return content.ToJArray().ToString();
		}

		/// <summary>
		/// Converts a collection of SimpleContent instances to a JArray.
		/// </summary>
		/// <param name="content">The SimpleContent collection you wish to convert.</param>
		/// <returns>JArray representation of the specified SimpleContent collection.</returns>
		public static JArray ToJArray(this IEnumerable<SimpleContent> content) {
			return JArray.FromObject(content);
		}
	}
}
