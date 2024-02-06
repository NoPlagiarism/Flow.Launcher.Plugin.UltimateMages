using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using Flow.Launcher.Plugin;
using Mages.Core;
using Mages.Plugins.LinearAlgebra;

namespace Flow.Launcher.Plugin.UltimateMages
{
    public class UltimateMages : IPlugin
    {
        private PluginInitContext _context;
        private static Engine MagesEngine;

        public void Init(PluginInitContext context)
        {
            _context = context;
            MagesEngine = new Engine(new Configuration
            {
                Scope = new Dictionary<string, object>
                {
                    {"e", Math.E}, // e is not contained in the default mages engine
                }
            });
            MagesEngine.AddPlugin(typeof (LinearAlgebraPlugin));
        }

        public List<Result> Query(Query query)
        {
            try
            {
                var rawResult = MagesEngine.Interpret(query.Search);
                string result;
                if (rawResult.GetType().IsArray)
                    result = string.Join(", ", Array.ConvertAll<object, string>((object[])rawResult, Convert.ToString));
                else if (rawResult.GetType().IsArray && ((Array)rawResult).Rank > 1)
                {
                    result = rawResult.ToString();
                }
                else
                    result = Convert.ToString(rawResult);
                /*switch (rawResult.GetType())
                {
                    case Array<object>:
                        string result = "LoL";
                        break;

                }*/
                return new List<Result> { new Result { Title = result, IcoPath = "icon.png", CopyText = result,
                                            Action = c => { try { _context.API.CopyToClipboard(result); return true; } 
                                                catch (ExternalException) { MessageBox.Show("Copy failed, please try later"); return false; }}
                }};
            }
            catch (Exception exc) {  }
            return new List<Result> { new Result { Title = "No result", IcoPath = "icon.png" } };
        }
    }
}