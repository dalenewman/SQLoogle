using System;
using System.Configuration;

namespace WebService.Auto
{
	public static class Config
	{
	
		public static String webpagesVersion
		{
			get
			{
				return ConfigurationManager.AppSettings["webpages:Version"];
			}
		}
	
		public static String ClientValidationEnabled
		{
			get
			{
				return ConfigurationManager.AppSettings["ClientValidationEnabled"];
			}
		}
	
		public static String UnobtrusiveJavaScriptEnabled
		{
			get
			{
				return ConfigurationManager.AppSettings["UnobtrusiveJavaScriptEnabled"];
			}
		}
	
		public static String SearchIndexPath
		{
			get
			{
				return ConfigurationManager.AppSettings["SearchIndexPath"];
			}
		}
	
		public static String SearchResultsLimit
		{
			get
			{
				return ConfigurationManager.AppSettings["SearchResultsLimit"];
			}
		}
	}
}

