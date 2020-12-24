using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

namespace Assets.Scripts.Common
{
    public class TemplateManager
    {
        public static void LoadAllTable()
        {
            using (var stream = new FileStream(Application.dataPath + "/Resources/Excel/table_gen.bin", FileMode.Open))
            {
                stream.Position = 0;

                var reader = new tabtoy.TableReader(stream);

                var tab = new main.Table();

                try
                {
                    tab.Deserialize(reader);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                // 表遍历
                foreach (var kv in tab.ExampleData)
                {
                    Debug.Log(kv.ID + "  " + kv.Name);
                }

                // 直接取值
                Debug.Log(tab.ExtendData[1].Additive);

                // KV配置
                Debug.Log(tab.GetKeyValue_ExampleKV().ServerIP);
            }
        }

        // 读取指定名字的表, 可根据实际需求调整该函数适应不同加载数据来源
        public static void LoadTableByName(main.Table tab, string tableName)
        {
            using (var stream = new FileStream(string.Format(Application.dataPath + "/Resources/Excel/{0}.bin", tableName), FileMode.Open))
            {
                stream.Position = 0;

                var reader = new tabtoy.TableReader(stream);
                try
                {
                    tab.Deserialize(reader);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        // 指定表读取例子
        public static void LoadSpecifiedTable()
        {
            var tabData = new main.Table();

            LoadTableByName(tabData, "ExampleData");
            LoadTableByName(tabData, "ExtendData");

            Debug.Log("Load table merged into one class");
            // 表遍历
            foreach (var kv in tabData.ExampleData)
            {
                Debug.Log(kv.ID + "  " + kv.Name);
            }
            // 表遍历
            foreach (var kv in tabData.ExtendData)
            {
                Debug.Log(kv.Additive);
            }

            Debug.Log("Load KV table into one class");
            var tabKV = new main.Table();
            LoadTableByName(tabKV, "ExampleKV");

            // KV配置
            Debug.Log(tabKV.GetKeyValue_ExampleKV().ServerIP);
        }
    }
}
