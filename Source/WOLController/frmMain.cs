using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bass.Util.WOL
{
    public partial class frmMain : Form
    {
        private WOLSender mSender = new WOLSender();
        private DataManager mDataManager = new DataManager();

        public frmMain()
        {
            InitializeComponent();
            _LoadData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (WOLData data in listMachineList.Items)
            {
                if (data.MachineName.Equals(txtMachineName.Text))
                {
                    if (data.MacAddress.Equals(txtMACAddress.Text.Trim()))
                    {
                        // exists
                        data.MacAddress = txtMACAddress.Text.Trim();
                        mDataManager.UpdateData(data);
                        return;
                    }
                }
            }

            // not exists
            WOLData addData = new WOLData();
            addData.MachineName = txtMachineName.Text;
            addData.MacAddress = txtMACAddress.Text.Trim();
            listMachineList.Items.Add(addData);
            mDataManager.InsertData(addData);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (-1 == listMachineList.SelectedIndex)
                return;

            WOLData data = listMachineList.SelectedItem as WOLData;
            if (null == data)
                return;

            listMachineList.Items.Remove(data);
            listMachineList.SelectedIndex = -1;

            mDataManager.DeleteData(data);
        }

        private void btnWake_Click(object sender, EventArgs e)
        {
            if (!WOLSender.IsValidMacAddress(txtMACAddress.Text))
            {
                MessageBox.Show("Invalid Mac Address Format!!");
                return;
            }

            mSender.Wake(txtMACAddress.Text);
        }




        private void _LoadData()
        {
            listMachineList.Items.Clear();

            var list = mDataManager.LoadDatas();

            foreach (var data in list)
            {
                listMachineList.Items.Add(data);
            }
        }

        private void listMachineList_SelectedIndexChanged(object sender, EventArgs e)
        {
            WOLData data = listMachineList.SelectedItem as WOLData;
            if(null == data)
            {
                txtMachineName.Text = string.Empty;
                txtMACAddress.Text = string.Empty;
            }
            else
            {
                txtMachineName.Text = data.MachineName;
                txtMACAddress.Text = data.MacAddress;
            }
        }
    }
}
