using System;
using System.Configuration;

namespace WebSite
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
	
		public static String SqloogleApi
		{
			get
			{
				return ConfigurationManager.AppSettings["SqloogleApi"];
			}
		}
	
		public static String StaticContentVersion
		{
			get
			{
				return ConfigurationManager.AppSettings["StaticContentVersion"];
			}
		}
		public static String ApplicationServices
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
			}
		}
	}
}

