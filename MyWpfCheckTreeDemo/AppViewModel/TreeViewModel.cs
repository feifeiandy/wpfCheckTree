using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows;

namespace MyWpfCheckTreeDemo.AppViewModel
{
    public class TreeViewModel:NotifyPropertyBase
    {
        public List<MyTree> MyTrees
        {
            get;
            set;
        }
        public TreeViewModel()
        {
            MyTrees = new List<MyTree>();
            MyTrees.Add(MyCreateTree());
            
        }
        /// <summary>
        /// 创建树
        /// </summary>
        /// <returns></returns>
        public MyTree MyCreateTree()
        {
            MyTree _myT = new MyTree("中国");
            #region 北京
            MyTree _myBJ = new MyTree("北京");
            _myT.CreateTreeWithChildre(_myBJ, false);
            MyTree _HD = new MyTree("海淀区");
            
            
            MyTree _CY = new MyTree("朝阳区");
            MyTree _FT = new MyTree("丰台区");
            MyTree _DC = new MyTree("东城区");

            _myBJ.CreateTreeWithChildre(_HD, false);
            _HD.CreateTreeWithChildre(new MyTree("某某1"), false);
            _HD.CreateTreeWithChildre(new MyTree("某某2"), true);
            _myBJ.CreateTreeWithChildre(_CY, false);
            _myBJ.CreateTreeWithChildre(_FT, false);
            _myBJ.CreateTreeWithChildre(_DC, false);
 
            #endregion

            #region 河北
            MyTree _myHB = new MyTree("河北");
            _myT.CreateTreeWithChildre(_myHB, false);
            MyTree _mySJZ = new MyTree("石家庄");
            MyTree _mySD = new MyTree("山东");
         
            MyTree _myTS = new MyTree("唐山");

            _myHB.CreateTreeWithChildre(_mySJZ, true);
            _myHB.CreateTreeWithChildre(_mySD, false);
            _myHB.CreateTreeWithChildre(_myTS, false);
            #endregion

            return _myT;
        }

        
    }

    /// <summary>
    /// 因为用到泛型了不能写成abstract 类
    /// 
    /// </summary>
    public  class MyTree : NotifyPropertyBase
    {

 
        #region 父
        public MyTree Parent
        {
            get;
            set;
        }
        #endregion

        #region 子
        public List<MyTree> Children
        {
            get;
            set;
        }
        #endregion

        #region 节点的名字
        public string Name
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public MyTree(string name)
        {
            this.Name=name;
            this.Children=new List<MyTree>();
        }
        public MyTree() { }

        public 
        #endregion

        #region CheckBox是否选中
        bool? _isChecked;
        public bool? IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                SetIsChecked(value, true, true);
            }
        }

        private void SetIsChecked(bool? value, bool checkedChildren, bool checkedParent)
        {
            if (_isChecked == value) return;
            _isChecked = value;
            //选中和取消子类
            if (checkedChildren && value.HasValue && Children != null)
                Children.ForEach(ch => ch.SetIsChecked(value, true, false));

            //选中和取消父类
            if (checkedParent && this.Parent != null)
                this.Parent.CheckParentCheckState();

            //通知更改
            
            this.SetProperty(x => x.IsChecked);
        }

        /// <summary>
        /// 检查父类是否选 中
        /// 如果父类的子类中有一个和第一个子类的状态不一样父类ischecked为null
        /// </summary>
        private void CheckParentCheckState()
        {
            bool? _currentState = this.IsChecked;
            bool? _firstState = null;
            for (int i = 0; i < this.Children.Count(); i++)
            {
                bool? childrenState = this.Children[i].IsChecked;
                if (i == 0)
                {
                    _firstState = childrenState;
                }
                else if (_firstState != childrenState)
                {
                    _firstState = null;
                }
            }
            if (_firstState != null) _currentState = _firstState;
            SetIsChecked(_firstState, false, true);
        }

        #endregion

        #region 选中的行 IsSelected
        bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                this.SetProperty(x => x.IsChecked);
                if (_isSelected)
                {
                    SelectedTreeItem = this;
                    MessageBox.Show("选中的是" + SelectedTreeItem.Name);
                }
                else
                    SelectedTreeItem = null;
            }
        }
        #endregion

        #region 选中的数据
        public MyTree SelectedTreeItem
        {
            get;
            set;
        }
        #endregion

        #region 创建树

        public void CreateTreeWithChildre( MyTree children,bool? isChecked)
        {
            this.Children.Add(children);

            children.Parent = this;
            children.IsChecked = isChecked;
        }
        #endregion
    }

    #region  INotifyPropertyChanged
    /// <summary>
    /// 
    /// </summary>
    public class NotifyPropertyBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    /// <summary>
    /// 扩展方法
    /// 避免硬编码问题
    /// </summary>
    public static class NotifyPropertyBaseEx
    {
        public static void SetProperty<T, U>(this T tvm, Expression<Func<T, U>> expre) where T : NotifyPropertyBase, new()
        {
            string _pro = CommonFun.GetPropertyName(expre);
            tvm.OnPropertyChanged(_pro);
        }
    }
    #endregion


    public class CommonFun
    {
        /// <summary>
        /// 返回属性名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string GetPropertyName<T, U>(Expression<Func<T, U>> expr)
        {
            string _propertyName = "";
            if (expr.Body is MemberExpression)
            {
                _propertyName = (expr.Body as MemberExpression).Member.Name;
            }
            else if (expr.Body is UnaryExpression)
            {
                _propertyName = ((expr.Body as UnaryExpression).Operand as MemberExpression).Member.Name;
            }
            return _propertyName;
        }
    }
}
