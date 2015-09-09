using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models.Object
{
    public class Device 
    {
        public string platform { get; set; }
        public string model { get; set; }
        public string uuid { get; set; }
        public string version { get; set; }
        public Int16 screen_width { get; set; }
        public Int16 screen_height { get; set; }
    }

    public class Push
    {
        public List<string> android_tokens { get; set; }
        public List<string> ios_tokens { get; set; }
        //{"android_tokens": ["TOKEN", "LIST"]}

        public Push() 
        { 
            this.android_tokens = new List<string>();
            this.ios_tokens = new List<string>();
        }
    }

    public class IonicWebHook 
    {
        public Device device { get; set; }
        public string name { get; set; }
        public string usuario { get; set; }
        public string user_id { get; set; }
        public Boolean unregister { get; set; }
        public string app_id { get; set; }
        public Boolean is_on_device { get; set; }
        public string received { get; set; }
        public string bio { get; set; }
        public Push _push { get; set; }

        public IonicWebHook() 
        {
            this.device = new Device();
            this._push = new Push();
        }
    }
    /*
        {
          "device": 
            {
              "platform": "Android", 
              "model": "MOBILE_MODEL", 
              "uuid": "DEVICE_UUID", 
              "version": "5.0.1", 
              "screen_width": 1080, 
              "screen_height": 1920
             }, 
          "_push": {"android_tokens": ["TOKEN", "LIST"]},
          "name": "Ionitron", 
          "user_id": "USER_ID", 
          "unregister": false, 
          "app_id": "YOUR_APP_ID",
          "is_on_device": true,
          "received": "2015-07-27T18:20:30.491924", 
          "bio": "I come from planet Ion"
        }
    */
    /*
    public class Autenticado
    {
        public string nome { get; set; }
        public string usuario { get; set; }
        public string token { get; set; }
        public Int32 id_grupo { get; set; }
        public string nu_cnpj { get; set; }
        public Boolean filtro_empresa { get; set; }
        public List<dynamic> controllers { get; set; }
    }


    public class Controllers 
    {
        public int id_controller { get; set; }
        public string ds_controller { get; set; }
        public List<dynamic> subControllers { get; set; }
        public Boolean home { get; set; }
    }
    */

}