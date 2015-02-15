<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/main.Master" AutoEventWireup="true" CodeBehind="Network.aspx.cs" Inherits="MarcelJoachimKloubert.CLRDashboard.Network" %>

<%@ Import Namespace="System.Net.NetworkInformation" %>

<asp:Content ID="Content_Main" ContentPlaceHolderID="ContentPlaceHolder_Main" runat="server">
    <style type="text/css">
        #cdbNetworkInterfaces th.cdbType {
            width: 20em;
        }

        #cdbNetworkInterfaces th.cdbMAC {
            width: 20em;
        }
    </style>

    <script type="text/javascript">

        $jsTB.page.funcs.showNetInterfaceDetails = function (id) {
            $jsTB.funcs.openPopup('NetInterfaceInfo.aspx?id=' + encodeURIComponent(id),
                                  'cdbNetInterface' + id);
        };

    </script>

    <ol class="breadcrumb">
      <li><a href="Default.aspx">Home</a></li>
      <li class="active">Network</li>
    </ol>

    <div class="panel panel-info">
      <div class="panel-heading">Network interfaces</div>

      <div class="panel-body">
    <%
        try
        {
            var netInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            if (netInterfaces.Length > 0)
            {
    %>
                <ul class="list-group">
                  <%
                        
                  foreach (var ni in netInterfaces.OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase))
                  {
                      var id = ni.Id;
                      var name = ni.Name;
                      var mac = ni.GetPhysicalAddress();
                      var type = ni.NetworkInterfaceType;
                      var desc = ni.Description;

                      var cssClasses = new List<string>();
                      switch(ni.OperationalStatus)
                      {
                          case OperationalStatus.Down:
                              cssClasses.Add("list-group-item-warning");
                              break;

                          case OperationalStatus.Up:
                              cssClasses.Add("list-group-item-success");
                              break;
                      }
                    %>
                        <li class="list-group-item <%= string.Join(" ", cssClasses) %>">
                          <h4 class="list-group-item-heading"><a href="#" onclick="$jsTB.page.funcs.showNetInterfaceDetails('<%= HttpUtility.JavaScriptStringEncode(id) %>')"><%= HttpUtility.HtmlEncode(name) %></a></h4>
                          <p class="list-group-item-text"><%= HttpUtility.HtmlEncode(desc) %></p>
                        </li>
                    <%
                  }
                        
                    %>
                </ul>
    <%
            }
            else
            {
                
            }
        }
        catch (Exception ex)
        {
            
        }
    %>
      </div>
    </div>
</asp:Content>