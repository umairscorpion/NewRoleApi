using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Integration.Core.Notification.Interface { 

    public interface ISettingProvider<T>
	{
		T GetSetting();
	}
}
