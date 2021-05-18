using RandomizerCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SekiroRandomizer.Patch
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var assembly = Assembly.LoadFrom("SekiroRandomizer.exe");
            var tProgram = assembly.GetType("SekiroRandomizer.Program");
            var mMain = tProgram.GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static);

            LocalizeSekiroForm();

            mMain.Invoke(null, new object[] { args });
        }

        private static async void LocalizeSekiroForm()
        {
            SekiroForm form = await Task.Run(() =>
            {
                do
                    Task.Delay(100);
                 while ((Application.OpenForms[nameof(SekiroForm)] as SekiroForm) is null);
                return Application.OpenForms[nameof(SekiroForm)] as SekiroForm;
            });

            form.Invoke(new Action(() =>
            {
                form.Text = "只狼 敌人和道具随机MOD " + Regex.Match(form.Text, @"(?<= )v[\d.]+").Value;
                form.MaximizeBox = false;

                void patchButtonClickEvent<T>(Button button, T patch) where T : Delegate
                {
                    // Use reflection technology in order to add patch method to Button's Click event.
                    var entry = typeof(EventHandlerList).GetMethod("Find", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(
                        typeof(Button).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(button),
                        new object[] { typeof(Control).GetField("EventClick", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) }
                    ); // Get ListEntry for Button's Click event implemented by Control class.
                    var fHandler = typeof(EventHandlerList).GetNestedType("ListEntry", BindingFlags.NonPublic).GetField("handler", BindingFlags.NonPublic | BindingFlags.Instance); // Get 'handler' FieldInfo in order to get/set event delegate.
                    // Place patch method first when combining delegates.
                    fHandler.SetValue(entry,
                        Delegate.Combine(
                            patch,
                            (Delegate)fHandler.GetValue(entry)
                        )
                    );
                }

                var preset = form.Controls["preset"] as Button;
                preset.Text = "选择挑战预设";
                patchButtonClickEvent(preset, (EventHandler)((sender, e) => LocalizePresetForm()));

                var optionwindow = form.Controls["optionwindow"] as Button;
                optionwindow.Text = "输入选项字符串";
                patchButtonClickEvent(optionwindow, (EventHandler)((sender, e) => LocalizeOptionsForm()));

                var enemyGroup = form.Controls["enemyGroup"];
                var enemy = enemyGroup.Controls["enemy"];
                enemy.Text = "敌人随机选项";
                var groupBox1 = enemyGroup.Controls["groupBox1"];
                groupBox1.Text = "随机选项";
                var bosses = groupBox1.Controls["bosses"];
                bosses.Text = "随机BOSS";
                var dlc1L = groupBox1.Controls["dlc1L"];
                dlc1L.Text = "替换游戏中的BOSS及其血条";
                var minibosses = groupBox1.Controls["minibosses"];
                minibosses.Text = "随机小BOSS";
                var dlc2L = groupBox1.Controls["dlc2L"];
                dlc2L.Text = "替换游戏中的小BOSS及其血条";
                var headlessmove = groupBox1.Controls["headlessmove"];
                headlessmove.Text = "随机无首";
                var label3 = groupBox1.Controls["label3"];
                label3.Text = "无首会作为小BOSS参与随机";
                var enemies = groupBox1.Controls["enemies"];
                enemies.Text = "随机小怪";
                var label4 = groupBox1.Controls["label4"];
                label4.Text = "替换游戏中的小怪";

            }));
        }

        private static async void LocalizePresetForm()
        {
            PresetForm form = await Task.Run(() =>
            {
                do
                    Task.Delay(100);
                while ((Application.OpenForms[nameof(PresetForm)] as PresetForm) is null);
                return Application.OpenForms[nameof(PresetForm)] as PresetForm;
            });

            form.Invoke(new Action(() =>
            {
                form.Text = "预设选项";

                var label3 = form.Controls["label3"];
                label3.Text = "预设项目均从“presets”文件夹下加载。\n" +
                    "预设项目在普通随机并不需要设置。\n" +
                    "预设项目主要用于挑战、竞速和展示。";
                var label1 = form.Controls["label1"];
                label1.Text = "预设：";
                var label2 = form.Controls["label2"];
                label2.Text = "敌人：";
                var submit = form.Controls["submit"];
                submit.Text = "选择";
                var desc = form.Controls["desc"];
                desc.Text = "默认选项，所有敌人都在各自随机池内进行随机。";
                typeof(PresetForm).GetField("defaultText", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(form, desc.Text);

                var select = form.Controls["select"] as ComboBox;
                var fPresetNames = typeof(PresetForm).GetField("presetNames", BindingFlags.NonPublic | BindingFlags.Instance);
                BindingList<string> presetNames = new BindingList<string>(fPresetNames.GetValue(form) as List<string>);
                if (presetNames[0] == "(None)") presetNames[0] = "（无）";
                select.DataSource = presetNames;
            }));
            ;
        }

        private static async void LocalizeOptionsForm()
        {

        }
    }
}
