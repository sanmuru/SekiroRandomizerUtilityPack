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

            LocalizeSekiroForm(assembly);

            mMain.Invoke(null, new object[] { args });
        }

        private static async void LocalizeSekiroForm(Assembly assembly)
        {
            Form form = await Task.Run(() =>
            {
                do
                    Task.Delay(100);
                 while ((Application.OpenForms["SekiroForm"]) is null);
                return Application.OpenForms["SekiroForm"];
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
                patchButtonClickEvent(preset, (EventHandler)((sender, e) => LocalizePresetForm(assembly)));

                var optionwindow = form.Controls["optionwindow"] as Button;
                optionwindow.Text = "输入选项字符串";
                patchButtonClickEvent(optionwindow, (EventHandler)((sender, e) => LocalizeOptionsForm(assembly)));

                #region enemyGroup
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
                var groupBox3 = enemyGroup.Controls["groupBox3"];
                groupBox3.Text = "扩展选项";
                var phases = groupBox3.Controls["phases"];
                phases.Text = "BOSS总阶段数保持相似";
                var weaponprogressionL = groupBox3.Controls["weaponprogressionL"];
                weaponprogressionL.Text = "例如防止剑圣一心和怨恨之鬼的车轮战";
                var phasebuff = groupBox3.Controls["phasebuff"];
                phasebuff.Text = "早期BOSS总阶段数应该小于终战BOSS总阶段数";
                var estusprogressionL = groupBox3.Controls["estusprogressionL"];
                estusprogressionL.Text = "防止因未经调整的早期BOSS过于强大而丧失游戏体验";
                var earlyreq = groupBox3.Controls["earlyreq"];
                earlyreq.Text = "早期小BOSS更加容易";
                var soulsprogressionL = groupBox3.Controls["soulsprogressionL"];
                soulsprogressionL.Text = "防止因未经调整的早期小BOSS过于强大而丧失游戏体验";
                var scale = groupBox3.Controls["scale"];
                scale.Text = "调高/低敌人的血量/伤害";
                var label13 = groupBox3.Controls["label13"];
                label13.Text = "当敌人被大幅移动到早/后期时调整对应难易度";
                #endregion
                #region itemGroup
                var itemGroup = form.Controls["itemGroup"];
                var item = itemGroup.Controls["item"];
                item.Text = "道具随机选项";
                var groupBox4 = itemGroup.Controls["groupBox4"];
                groupBox4.Text = "偏好选项";
                var difficultyL = groupBox4.Controls["difficultyL"] as Label;
                difficultyL.TextChanged += (sender, e) => Localize_difficultyL(sender as Label);
                Localize_difficultyL(difficultyL);
                var groupBox2 = itemGroup.Controls["groupBox2"];
                groupBox2.Text = "关键物品随机？";
                var defaultA = groupBox2.Controls["defaultA"];
                defaultA.Text = "所有地点";
                var norandom = groupBox2.Controls["norandom"];
                norandom.Text = "不随机";
                norandom.Location = new Point(212, 21);
                var racemode = groupBox2.Controls["racemode"];
                racemode.Text = "重要地点";
                var groupBox7 = itemGroup.Controls["groupBox7"];
                groupBox7.Text = "战斗记忆随机？";
                var defaultB = groupBox7.Controls["defaultB"];
                defaultB.Text = "所有地点";
                var norandom_dmg = groupBox7.Controls["norandom_dmg"];
                norandom_dmg.Text = "不随机";
                norandom_dmg.Location = new Point(212, 21);
                var racemode_dmg = groupBox7.Controls["racemode_dmg"];
                racemode_dmg.Text = "重要地点";
                var weaponprogression = groupBox7.Controls["weaponprogression"];
                weaponprogression.Text = "战斗记忆用于提升攻击力，功能同原游戏";
                var groupBox9 = itemGroup.Controls["groupBox9"];
                groupBox9.Text = "技能/忍具技能随机？";
                var defaultD = groupBox9.Controls["defaultD"];
                defaultD.Text = "所有地点";
                var norandom_skills = groupBox9.Controls["norandom_skills"];
                norandom_skills.Text = "不随机";
                norandom_skills.Location = new Point(212, 21);
                var racemode_skills = groupBox9.Controls["racemode_skills"];
                racemode_skills.Text = "重要地点";
                var skillprogression = groupBox9.Controls["skillprogression"];
                skillprogression.Text = "通常在遭遇敌人特技障碍前可以获得";
                var splitskills = groupBox9.Controls["splitskills"];
                splitskills.Text = "通过拾取代替技能学习";
                var groupBox8 = itemGroup.Controls["groupBox8"];
                groupBox8.Text = "佛珠/葫芦种子随机？";
                var defaultC = groupBox8.Controls["defaultC"];
                defaultC.Text = "所有地点";
                var norandom_health = groupBox8.Controls["norandom_health"];
                norandom_health.Text = "不随机";
                norandom_health.Location = new Point(212, 21);
                var racemode_health = groupBox8.Controls["racemode_health"];
                racemode_health.Text = "重要地点";
                var healthprogression = groupBox8.Controls["healthprogression"];
                healthprogression.Text = "佛珠/种子用于提升体力/血量，功能同原游戏";
                #region groupBox6
                var groupBox6 = itemGroup.Controls["groupBox6"];
                groupBox6.Text = "其他道具选项";
                var headlessignore = groupBox6.Controls["headlessignore"];
                headlessignore.Text = "不必要打无首";
                var label7 = groupBox6.Controls["label7"];
                label7.Text = "无首不会掉落关键道具（需要取消“随机无首”）";
                var carpsanity = groupBox6.Controls["carpsanity"];
                carpsanity.Text = "宝鲤精神";
                var label6 = groupBox6.Controls["label6"];
                label6.Text = "宝鲤之鳞的掉落也会随机";
                var earlyhirata = groupBox6.Controls["earlyhirata"];
                earlyhirata.Text = "提前进入平田宅邸";
                var label9 = groupBox6.Controls["label9"];
                label9.Text = "在赤鬼房间前必获得神子的守护铃";
                var veryearlyhirata = groupBox6.Controls["veryearlyhirata"];
                veryearlyhirata.Text = "允许初期进入平田宅邸";
                var label10 = groupBox6.Controls["label10"];
                label10.Text = "忍义手可能会出现在平田宅邸（不软锁定）";
                #endregion
                #endregion
                var enemytoitem = form.Controls["enemytoitem"];
                enemytoitem.Text = "技能/忍具技能随着随机敌人的位置放置";
                var label1 = form.Controls["label1"];
                label1.Text = "如果狮子猿是第一个BOSS，那你可以更早地获取长枪忍具。\r\n" +
                    "使道具随机逻辑更贴近敌人随机逻辑";
                var defaultAllowReroll = form.Controls["defaultAllowReroll"];
                defaultAllowReroll.Text = "允许中途重新随机敌人而不影响";
                var label12 = form.Controls["label12"];
                label12.Text = "道具随机逻辑和敌人随机逻辑完全独立";
                var mergemods = form.Controls["mergemods"];
                mergemods.Text = "从常用的“mods”文件夹中合并MOD";
                var headlesswalk = form.Controls["headlesswalk"];
                headlesswalk.Text = "取消无首的减速光环";
                var edittext = form.Controls["edittext"];
                edittext.Text = "更新游戏内的文本（重命名BOSS和添加提示）";
                var openstart = form.Controls["openstart"];
                openstart.Text = "打开种鬼佛堂通往仙峰寺的门";
                var label8 = form.Controls["label8"];
                label8.Text = "固定种子";
                var label2 = form.Controls["label2"];
                label2.Text = "敌人种子";
                var defaultReroll = form.Controls["defaultReroll"];
                defaultReroll.Text = "新的种子";
                var defaultRerollEnemy = form.Controls["defaultRerollEnemy"];
                defaultRerollEnemy.Text = "新的敌人种子";

                var randomize = form.Controls["randomize"];
                randomize.Text = "开始新的随机";
            }));
        }

        private static void Localize_difficultyL(Label difficultyL)
        {
            var lines = difficultyL.Text.Split(new[] { "\r\n" }, StringSplitOptions.None);
            switch (lines[0])
            {
                case "All locations for items are equally likely. Often results in a lot of early memories and prayer beads.":
                    lines[0] = "在所有位置上道具放置概率相同。经常在前期能拾取到大量战斗记忆和佛珠。";
                    break;
                case "Most locations for items are equally likely. Often results in a lot of early memories and prayer beads.":
                    lines[0] = "在大多数位置上道具的放置概率相同。经常在前期能拾取到大量战斗记忆和佛珠。";
                    break;
                case "Slightly better rewards for difficult and late locations.":
                    lines[0] = "难以到达、后期的位置获得的收益稍微提高。";
                    break;
                case "Better rewards for difficult and late locations.":
                    lines[0] = "难以到达、后期的位置获得的收益可观提高。";
                    break;
                case "Much better rewards for difficult and late locations.":
                    lines[0] = "难以到达、后期的位置获得的收益很大提高。";
                    break;
            }
            switch (lines[1])
            {
                case "Key items will usually be in different areas and form interesting chains.":
                    lines[1] = "关键道具往往分布在不同区域，并且形成有意思的先后顺序。";
                    break;
                case "Key items will usually be in different areas and depend on each other.":
                    lines[1] = "关键道具往往分布在不同区域，并且形成相互依赖。";
                    break;
                case "Key items will usually be easy to find and not require much side content.":
                    lines[1] = "关键道具往往容易找到，并没有过多的附加内容。";
                    break;
            }
            difficultyL.Text = lines[0] + "\r\n" + lines[1];
        }

        private static async void LocalizePresetForm(Assembly assembly)
        {
            Form form = await Task.Run(() =>
            {
                do
                    Task.Delay(100);
                while ((Application.OpenForms["PresetForm"]) is null);
                return Application.OpenForms["PresetForm"];
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
                Type tPresetFrom = assembly.GetType("RandomizerCommon.PresetForm");
                tPresetFrom.GetField("defaultText", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(form, desc.Text);

                var select = form.Controls["select"] as ComboBox;
                var fPresetNames = tPresetFrom.GetField("presetNames", BindingFlags.NonPublic | BindingFlags.Instance);
                BindingList<string> presetNames = new BindingList<string>(fPresetNames.GetValue(form) as List<string>);
                if (presetNames[0] == "(None)") presetNames[0] = "（无）";
                select.DataSource = presetNames;
                var enemy = form.Controls["enemy"] as ComboBox;
                enemy.Text = "（无）";
            }));
            ;
        }

        private static async void LocalizeOptionsForm(Assembly assembly)
        {
            Form form = await Task.Run(() =>
            {
                do
                    Task.Delay(100);
                while ((Application.OpenForms["OptionsForm"]) is null);
                return Application.OpenForms["OptionsForm"];
            });

            form.Invoke(new Action(() =>
            {
                form.Text = "字符串选项";
                var label1 = form.Controls["label1"];
                label1.Text = "下方文本框让你能够将自己的随机设置（在随机之后）分享给他人或是粘贴他人的设置。\r\n" +
                    "这段字符串也能在“spoiler_logs”文件夹下的提示/混淆日志的第一行中找到。\r\n" +
                    "另外，你还可以通过将它清空来恢复所有默认设置。";
                var select = form.Controls["select"];
                select.Text = "选择";
            }));
        }
    }
}
