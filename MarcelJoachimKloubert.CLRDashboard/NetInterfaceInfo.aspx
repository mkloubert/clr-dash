<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/popup.Master" AutoEventWireup="true" CodeBehind="NetInterfaceInfo.aspx.cs" Inherits="MarcelJoachimKloubert.CLRDashboard.NetInterfaceInfo" %>

<asp:Content ID="Content_Main" ContentPlaceHolderID="ContentPlaceHolder_Main" runat="server">
    <div class="panel panel-info">
      <div class="panel-heading">Network interface info</div>

      <div class="panel-body">
    <%
        if (this.NetworkInterface != null)
        {
    %>
          <table class="table table-striped table-hover">
              <tr>
                  <td>Name:</td>
                  <td><%= HttpUtility.HtmlEncode(this.NetworkInterface.Name) %></td>
              </tr>

              <tr>
                  <td>ID:</td>
                  <td><%= HttpUtility.HtmlEncode(this.NetworkInterface.Id) %></td>
              </tr>
          </table>
    <%
        }
        else
        {
            %>Interface not found!<%
        }
    %>
      </div>
    </div>

    <%
        if (this.NetworkInterface != null)
        {
            var stats = this.NetworkInterface.GetIPv4Statistics();
    %>

    <script type="text/javascript">

        $jsTB.page.addOnLoaded(function() {
            $jsTB.page.addElement('bytesReceived', '#cdbBytesReceived')
                      .addElement('bytesSent', '#cdbBytesSent');

            setTimeout($jsTB.page.funcs.updateStats,
                       2500);
        });

        $jsTB.page.funcs.updateStats = function () {
            $jsTB.$.ajax({
                url: 'stats/net_interface',
                type: 'POST',
                data: {
                    'id': <%= HttpUtility.JavaScriptStringEncode(this.NetworkInterface.Id, true) %>,
                },

                success: function(data) {
                    switch (data.code) {
                        case 0:
                            {
                                $jsTB.page.elements.bytesReceived
                                                   .text(data.data.bytes.received);

                                $jsTB.page.elements.bytesSent
                                                   .text(data.data.bytes.sent);
                            }
                            break;
                    }
                },

                complete: function() {
                    setTimeout($jsTB.page.funcs.updateStats,
                               5000);
                },
            });
        };

    </script>

    <div class="panel panel-info">
      <div class="panel-heading">Statistics</div>

      <div class="panel-body">
          <table class="table table-striped table-hover">
              <tr>
                  <td>Bytes received:</td>
                  <td id="cdbBytesReceived">---</td>
              </tr>

              <tr>
                  <td>Bytes send:</td>
                  <td id="cdbBytesSent">---</td>
              </tr>
          </table>
      </div>
    </div>

    <%
        }
    %>
</asp:Content>
