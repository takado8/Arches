using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Arches.viewModel
{
    internal interface ITreeViewItemSelected
    {
        public void treeViewChildItemSelected(TreeViewItem treeViewItem);
    }
}
