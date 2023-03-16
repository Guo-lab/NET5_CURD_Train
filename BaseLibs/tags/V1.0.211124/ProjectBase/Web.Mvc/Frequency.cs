using System;
namespace ProjectBase.Web.Mvc
{

    /**
	 * 标记对Action进行访问频度检查
	 * @author Rainy
	 * @see --basic
	 */
    [AttributeUsage(AttributeTargets.Method, Inherited=true,AllowMultiple =false)]
	public class FrequencyAttribute : Attribute
	{
		/**
		 * 接口访问最短间隔时间，毫秒。
		 */
		public int Interval { get; set; }
		/**
		 * 一次会话中的接口访问次数。
		 */
		public int TimesPerSession { get; set; }
		/**
		 * 一定时间段内的接口访问次数。
		 */
		public int Times { get; set; }
		/**
		 * 限制次数的时间段,多少天之内 。
		 */
		public int Days { get; set; }
		/**
		 * 是否进行检查
		 * @return
		 */
		public bool ShouldCheck { get; set; }= true;

		public FrequencyAttribute(bool shouldCheck)
		{
			ShouldCheck = shouldCheck;
		}
	}
}
