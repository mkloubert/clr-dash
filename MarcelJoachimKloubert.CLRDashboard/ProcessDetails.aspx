<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProcessDetails.aspx.cs" Inherits="MarcelJoachimKloubert.CLRDashboard.ProcessDetails" MasterPageFile="~/Masters/popup.Master" %>

<%@ Import Namespace="System.IO" %>


<asp:Content ID="Content_Main" ContentPlaceHolderID="ContentPlaceHolder_Main" Runat="Server">
    <div class="panel panel-info">
      <div class="panel-heading">Process info</div>

      <div class="panel-body">
<% if (this.CurrentProcess != null) { %>
          <script type="text/javascript">
              $jsTB.page.addOnLoaded(function () {
                  $jsTB.page.addElement('checkForVirusButton', '#cdbCheckForVirusBtn')
                            .addElement('virusTotalPermaLink', '#cdbVirusTotalPermaLink');

                  $jsTB.page.addVar('processId', <%= this.GetProcessValue((p) => p.Id, -1) %> );
              });
          </script>

          <table class="table table-striped table-hover">
              <tr>
                  <td>Name:</td>
                  <td><%= HttpUtility.HtmlEncode(this.GetProcessValue(p => p.ProcessName)) %></td>
              </tr>

              <tr>
                  <td>ID:</td>
                  <td><%= HttpUtility.HtmlEncode(this.GetProcessValue(p => p.Id)) %></td>
              </tr>

              <tr>
                  <td>Directory:</td>
                  <td><%= HttpUtility.HtmlEncode(this.GetProcessValue(p => Path.GetDirectoryName(p.MainModule.FileName))) %></td>
              </tr>

              <tr>
                  <td>File:</td>
                  <td><%= HttpUtility.HtmlEncode(this.GetProcessValue(p => Path.GetFileName(p.MainModule.FileName))) %></td>
              </tr>
          </table>
<% } %>
<% else { %>
          Process not found!
<% } %>
      </div>
    </div>

<% if (this.CurrentProcess != null) { %>
    <script type="text/javascript">
        $jsTB.page.funcs.checkForVirus = function () {
            $jsTB.$.ajax({
                url: 'actions/check_process_for_virus',
                type: 'POST',
                data: {
                    'pid': $jsTB.page.vars.processId,
                },

                beforeSend: function() {
                    $jsTB.page.elements.checkForVirusButton
                                       .prop('disabled', true);
                },

                success: function(data) {
                    switch (data.code) {
                        case 0:
                            window.open(data.data.link, '_blank');
                            break;
                    }
                },

                error: function() {

                },

                complete: function() {
                    $jsTB.page.elements.checkForVirusButton
                                       .prop('disabled', false);
                },
            });
        };
    </script>

    <div class="panel panel-info">
      <div class="panel-heading">Actions</div>

      <div class="panel-body">
          <a href="http://www.processlibrary.com/en/search/?q=<%= HttpUtility.UrlEncode(this.GetProcessValue(p => p.ProcessName)) %>" class="btn btn-info btn-lg btn-block" target="_blank">Search @ ProcessLibrary.com</a>

          <%
              var processFile = this.GetProcessValue((p) => p.MainModule.FileName);
              if (string.IsNullOrWhiteSpace(processFile) == false)
              {
                  if (File.Exists(processFile))
                  {
          %>
          <button id="cdbCheckForVirusBtn" type="button" onclick="$jsTB.page.funcs.checkForVirus()" class="btn btn-warning btn-lg btn-block">Virus check @ VirusTotal.com</button>
          <%          
                  }
              }
          %>
      </div>
    </div>      
<% } %>
</asp:Content>