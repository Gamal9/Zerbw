using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;
using Zerbow;
using Zerbow.Models;
using Zerbow.UWP;

[assembly: ExportRenderer(typeof(ExtMap), typeof(CustomRender))]

namespace Zerbow.UWP
{
  public  class CustomRender : MapRenderer
    {
        
      



        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);
        }
    }
}
