using Windows.System;
using AIPaste.Models.SettingsServices.SettingModels;

namespace AIPasteTests.Models.SettingsServices.SettingModels
{
    [TestClass()]
    public class KeyPatternTests
    {
        [TestMethod()]
        public void AnalyzeModifierTest()
        {
            var testKeyPattern = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);

            var result = testKeyPattern.AnalyzeModifier();

            var expected = new string[] { "Alt", "Ctrl" };
            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod()]
        public void GetKeyModifierFromStringTest()
        {
            var keyPatternString = new string[] { "Alt", "Ctrl" };
            
            var result = KeyPattern.GetKeyModifierFromString(keyPatternString);

            var expected = HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT;
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GetModifiersTest()
        {
            var result = KeyPattern.GetModifiers(true, true, false, false);

            var expected = HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT;
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            var testKeyPattern = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);

            var result = testKeyPattern.ToString();

            var expected = "Alt+Ctrl+C";
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void EqualsTest_returnTrue()
        {
            var testKeyPattern1 = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);
            var testKeyPattern2 = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);

            var result = testKeyPattern1.Equals(testKeyPattern2);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void EqualsTest_returnFalse()
        {
            var testKeyPattern1 = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);
            var testKeyPattern2 = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.V);
            
            var result = testKeyPattern1.Equals(testKeyPattern2);
            
            Assert.IsFalse(result);
        }
    }
}