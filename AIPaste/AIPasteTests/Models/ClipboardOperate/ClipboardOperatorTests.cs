using AIPaste.Models.ClipboardOperate;
using Windows.ApplicationModel.DataTransfer;

namespace AIPasteTests.Models.ClipboardOperate
{
    [TestClass()]
    public class ClipboardOperatorTests
    {
        [TestMethod()]
        public async Task GetTextAsyncTest()
        {
            var dummyString = "test";
            var dataPackage = new DataPackage();
            dataPackage.SetText(dummyString);
            var clipboardStub = new ClipboardStub(dataPackage);

            ClipboardOperator clipboardOperator = new ClipboardOperator(clipboardStub);
            var text = await clipboardOperator.GetTextAsync();

            Assert.AreEqual(dummyString, text);
        }

        [TestMethod()]
        public async Task SetTextTest()
        {
            var dummyString = "test";
            var dataPackage = new DataPackage();
            dataPackage.SetText(dummyString);
            var clipboardStub = new ClipboardStub(dataPackage);
            var setString = "set string";

            var clipboardOperator = new ClipboardOperator(clipboardStub);
            clipboardOperator.SetText(setString);
            var text = await clipboardOperator.GetTextAsync();

            Assert.AreEqual(setString, text);
        }

        [TestMethod()]
        public void RegisterContentChangedHandlerTest()
        {
            var dummyString = "test";
            var dataPackage = new DataPackage();
            dataPackage.SetText(dummyString);
            var clipboardStub = new ClipboardStub(dataPackage);
            var count = 0;
            var func = new EventHandler<object>((sender, e) => { count++; });

            var clipboardOperator = new ClipboardOperator(clipboardStub);
            clipboardOperator.RegisterContentChangedHandler(func);
            clipboardStub.RaiseContentChanged();
            clipboardStub.RaiseContentChanged();

            Assert.AreEqual(count, 2);
        }

        [TestMethod()]
        public void UnregisterContentChangedHandlerTest()
        {
            var dummyString = "test";
            var dataPackage = new DataPackage();
            dataPackage.SetText(dummyString);
            var clipboardStub = new ClipboardStub(dataPackage);
            var count = 0;
            var func = new EventHandler<object>((sender, e) => { count++; });

            var clipboardOperator = new ClipboardOperator(clipboardStub);
            clipboardOperator.RegisterContentChangedHandler(func);
            clipboardStub.RaiseContentChanged();
            clipboardOperator.UnregisterContentChangedHandler(func);
            clipboardStub.RaiseContentChanged();

            Assert.AreEqual(count, 1);
        }
    }

    public class ClipboardStub(DataPackage firstDataPackage) : IClipboardAccess
    {
        public event EventHandler<object>? ContentChanged;
        public DataPackage _virtualClipboard = firstDataPackage;
        public DataPackageView GetContent()
        {;
            return _virtualClipboard.GetView();
        }
        public void SetContent(DataPackage dataPackage)
        {
            _virtualClipboard = dataPackage;
        }

        public void RaiseContentChanged()
        {
            ContentChanged?.Invoke(this, new object());
        }
    }
}