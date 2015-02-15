<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/main.Master" AutoEventWireup="true" CodeBehind="Apps.aspx.cs" Inherits="MarcelJoachimKloubert.CLRDashboard.Apps" %>

<%@ Import Namespace="System.Diagnostics" %>

<asp:Content ID="Content_Main" ContentPlaceHolderID="ContentPlaceHolder_Main" runat="server">
    <style type="text/css">
        .cdbProcessItem img {
            margin-right: 1em;
            width: 2em;
        }
    </style>

    <script type="text/javascript">
        $jsTB.page.addOnLoaded(function () {
            $jsTB.page.funcs.loadNextProcessIcon();
        });

        $jsTB.page.funcs.loadNextProcessIcon = function () {
            if ($jsTB.page.vars.loadPorcessIconTasks.isEmpty()) {
                return;
            }

            var item = $jsTB.page.vars.loadPorcessIconTasks.pop();
            $jsTB.$.ajax({
                url: 'res/process_icon',
                type: 'POST',
                data: {
                    'pid': item.pid,
                },

                success: function (data) {
                    var iconSrc = null;

                    switch (data.code) {
                        case 0:
                            if (data.data != null) {
                                iconSrc = 'data:{0};base64,{1}'.format(data.data.mime,
                                                                       data.data.data);
                            }
                            break;
                    }

                    if (iconSrc != null) {
                        $jsTB.$(item.image).attr('src', iconSrc);
                    }
                },

                complete: function () {
                    $jsTB.page.funcs.loadNextProcessIcon();
                },
            });
        };

        $jsTB.page.funcs.openProcessInfo = function (pid) {
            $jsTB.funcs.openPopup('ProcessDetails.aspx?pid=' + encodeURIComponent(pid),
                                  {
                                      height: '500px',
                                      name: 'cdbProcessInfoPopup' + pid,
                                  });
        };

        $jsTB.page.addVar('loadPorcessIconTasks', []);
    </script>

    <ol class="breadcrumb">
      <li><a href="Default.aspx">Home</a></li>
      <li class="active">Apps</li>
    </ol>

    <div class="panel panel-info">
      <div class="panel-heading">Processes</div>

      <div class="panel-body">
          <%
              var processes = Process.GetProcesses();
              if (processes.Length > 0)
              {
                  foreach (var p in processes.OrderBy(x => GetValue(() => x.ProcessName), StringComparer.InvariantCultureIgnoreCase))
                  {
                      var pid = GetValue(() => p.Id, -1);
                      var anchor = string.Format("cdbProcess{0}", pid);
                      var elementId = string.Format("cdbProcessItem{0}", pid);
                      
                      %>
                          <a name="<%= anchor %>"></a>
                          <div class="media cdbProcessItem" id="<%= elementId %>">
                              <div class="media-left">
                                <a href="#">
                                  <img class="media-object" src="img/process.png">
                                </a>
                              </div>

                              <div class="media-body">
                                <h4 class="media-heading"><a href="#<%= anchor %>" onclick="$jsTB.page.funcs.openProcessInfo(<%= pid %>)"><%= HttpUtility.HtmlEncode(GetValue(() => p.ProcessName)) %></a></h4>
                                <%= HttpUtility.HtmlEncode(GetValue(() => p.MainModule.FileName)) %>
                              </div>
                          </div>

                          <script type="text/javascript">
                              $jsTB.page.vars.loadPorcessIconTasks.push({
                                  'image': '#<%= elementId %> img.media-object',
                                  'pid': <%= pid %>,
                              });
                          </script>
                      <%
                  }
              }
              else
              {
                  %>No processes found!<%
              }
          %>
      </div>
    </div>
</asp:Content>