# WindUI module
* WindUI is a lightweight MVVM UI framework, which realizes the complete separation of data logic and display logic.
* Developers only need to care about the binding of data and interface, and the realization of data logic can automatically drive the display logic of the UI.
* Provides a UGUI-based atlas management module. UIAtlasManager
* Provides a series of commonly used UGUI extension components.

## UI Data Binding
### ViewModel Container (ViewModelContainer)
* ![ui_1](/Doc/res/images/ui_1.png)
* Each UI prefab has a ViewModel container script (ViewModelContainer), and its ViewModelClass variable corresponds to a class on the hot update side, and a ViewModelContaner object is created when the UI is instantiated.
* ViewModels list, which stores multiple ViewModelDataSource objects.
* EventBindings list, store multiple event binding script objects.

### ViewModel Data Source (ViewModelDataSource)
* ![ui_2](/Doc/res/images/ui_2.png)
* This class is used to associate data and UI, and it is stored in ViewModelContainer.
* Create ViewModel objects by reflecting ViewModelPath.

### One-way binding (MemberBindingOneWay)
* ![ui_3](/Doc/res/images/ui_3.png)
* ViewPath variable: automatically find out the variables exposed to the outside world in all components under the current node.
* ViewModelPath variable: automatically find the variables in our custom ViewModel class according to the type of ViewPath.
* Through these two variables, the data can be bound to the UI.

### Two-way binding (MemberBindingTwoWay)
* ![ui_4](/Doc/res/images/ui_4.png)
* EventPath variable: automatically find out the event trigger interface exposed in all components in the current node, and monitor the value change of the component through event binding.
* View Path variable: automatically find out the variables exposed to the outside world in all components under the current node.
* ViewModelPath variable: automatically find the variables in our custom ViewModel class according to the type of ViewPath.

### EventBinding
* ![ui_5](/Doc/res/images/ui_5.png)
* ViewEvent variable: automatically find out the event trigger interface exposed in all components in the current node, and monitor the value change of the component through event binding.
* ViewModelMethod: automatically find out all methods in the ViewModel that contain binding tags.

### Hot update logic
* Add the DataBinding attribute tag to the logic class and attribute variable ‘method of the hot update terminal, which can be added under the Inspector.
* Add the HotfixBinding attribute tag to the variables in LoginView to automatically bind the corresponding ViewModel value in the ViewModelContainer.

```C#
    [DataBinding]
    public class LoginViewModel: ViewModel
    {
        private string mAccountName = "Test111";
        private string mPassword = "xxxxxxx";

        [DataBinding]
        public string AccountName
        {
            get {return mAccountName;}
            set
            {
                mAccountName = value;
                this.PropChanged("AccountName");
            }
        }

        [DataBinding]
        public string Password
        {
            get {return mPassword;}
            set
            {
                mPassword = value;
                this.PropChanged("Password");
            }
        }
    }

public class LoginView: ViewController
    {
        [HotfixBinding("Login")]
        public LoginViewModel ViewModel;

        protected override void OnOpening() { }
        
        protected override void OnUpdate() { }

        [DataBinding]
        private void OnBtnButton_Clicked()
        {
            ViewManager.Instance.Open("KNListTest", View.State.Dispatch);
        }
    }
```