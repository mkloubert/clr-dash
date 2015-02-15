<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MarcelJoachimKloubert.CLRDashboard.Default" MasterPageFile="~/Masters/main.Master" %>

<asp:Content ID="Content_Main" ContentPlaceHolderID="ContentPlaceHolder_Main" Runat="Server">
    <style type="text/css">
        #cdbDriveList table.table td.cdbName img.cdbIcon {
            height: 1.5em;
            margin-right: 0.5em;
            width: 1.5em;
        }

        #cdbDriveList table.table th.cdbName {
            width: 10em;
        }

        #cdbProcessList table.table td.cdbID {
            padding-right: 0.5em;
            text-align: center;
        }

        #cdbProcessList table.table td.cdbName {
            padding-left: 0.5em;
            padding-right: 0.5em;
        }

        #cdbProcessList table.table td.cdbName img.cdbIcon {
            height: 1.5em;
            margin-right: 0.5em;
            width: 1.5em;
        }

        #cdbProcessList table.table td.cdbActions {
            padding-left: 0.5em;
        }

        #cdbProcessList table.table td.cdbActions .btn {
            margin-left: 0.5em;
        }

        #cdbProcessList table.table th.cdbID {
            text-align: center;
            width: 10em;
        }

        #cdbProcessList table.table th.cdbActions {
            text-align: right;
            width: 20em;
        }
    </style>

    <script type="text/javascript">
        // load processes
        $jsTB.page.addOnLoaded(function () {
            $jsTB.page.addElement('cpuInfo', '#cdbCPUInfo');
            $jsTB.page.addElement('monitorList', '#cdbMonitorList');
            $jsTB.page.addElement('memInfo', '#cdbMemInfo');

            $jsTB.page.funcs.updateCpuInfo();
            $jsTB.page.funcs.updateMemInfo();

            $jsTB.page.funcs.reloadMonitors();
        });

        $jsTB.page.funcs.updateCpuInfo = function () {
            $jsTB.$.ajax({
                url: 'info/cpu',
                type: 'POST',

                success: function (data) {
                    var timeoutVal;

                    switch (data.code) {
                        case 0:
                            {
                                var newInfoBlock = $jsTB.$('<span><span>');
                                newInfoBlock.html('The system needs about <span class="cbCpuUsage">{0} %</span> of total CPU resources.'.format(parseInt(data.data.usage)));

                                $jsTB.page.elements.cpuInfo.html('')
                                                           .append(newInfoBlock);

                                timeoutVal = 1000;
                            }
                            break;

                        default:
                            timeoutVal = 15000;
                            break;
                    }

                    setTimeout($jsTB.page.funcs.updateCpuInfo, timeoutVal);
                },

                error: function () {
                    setTimeout($jsTB.page.funcs.updateCpuInfo, 60000);
                },
            });
        };

        $jsTB.page.funcs.updateMemInfo = function () {
            $jsTB.$.ajax({
                url: 'info/mem',
                type: 'POST',

                success: function (data) {
                    var timeoutVal;

                    switch (data.code) {
                        case 0:
                            {
                                var pMem = parseInt(data.data.available.physical / 1024.0 / 1024.0);
                                var vMem = parseInt(data.data.available.virtual / 1024.0 / 1024.0);
                                var tMem = parseInt((data.data.available.physical + data.data.available.virtual) / 1024.0 / 1024.0);

                                var newInfoBlock = $jsTB.$('<span><span>');
                                newInfoBlock.html('The system has about <span class="cbMemAvailable">{0} MB</span> physical and <span class="cbMemAvailable">{1} MB</span> virtual memory (<span class="cbMemAvailable">{2} MB</span> total) left.'.format(pMem, vMem, tMem));

                                $jsTB.page.elements.memInfo.html('')
                                                           .append(newInfoBlock);

                                timeoutVal = 1000;
                            }
                            break;

                        default:
                            timeoutVal = 15000;
                            break;
                    }

                    setTimeout($jsTB.page.funcs.updateMemInfo, timeoutVal);
                },

                error: function () {
                    setTimeout($jsTB.page.funcs.updateMemInfo, 60000);
                },
            });
        };

        $jsTB.page.funcs.reloadMonitors = function () {
            $jsTB.page.elements.monitorList.html('<img src="img/ajax_loader_128x15.gif" class="cdbAjaxLoader" />');

            $jsTB.$.ajax({
                url: 'lists/important_monitors',
                type: 'POST',

                success: function (data) {
                    switch (data.code) {
                        case 0:
                            {
                                $jsTB.page.elements.monitorList.html('');

                                for (var i = 0; i < data.data.length; i++) {
                                    var monitor = data.data[i];

                                    var newPanel = $jsTB.$('<div class="panel panel-default">' +
                                                           '<div class="panel-heading"></div>' + 
                                                           '<div class="panel-body"></div>' + 
                                                           '</div>');
                                    newPanel.find('.panel-heading').text(monitor.name);

                                    // items
                                    {
                                        var newList = $jsTB.$('<ul class="list-group"></ul>');

                                        for (var ii = 0; ii < monitor.items.length; ii++) {
                                            var monitorItem = monitor.items[ii];

                                            var newItem = $jsTB.$('<li class="list-group-item">' +
                                                                  '<h4 class="list-group-item-heading"></h4>' +
                                                                  '<p class="list-group-item-text"></p>' +
                                                                  '</li>');

                                            switch (monitorItem.state) {
                                                case 2:
                                                    newItem.addClass('list-group-item-warning');
                                                    break;

                                                case 3:
                                                    newItem.addClass('list-group-item-danger');
                                                    break;

                                                case 4:
                                                    newItem.addClass('danger');
                                                    newItem.css('font-weight', 'bold');
                                                    newItem.css('color', 'yellow');
                                                    break;
                                            }

                                            newItem.find('.list-group-item-heading').text(monitorItem.title);
                                            newItem.find('.list-group-item-text').text(monitorItem.description);

                                            newItem.appendTo(newList);
                                        }

                                        newList.appendTo(newPanel.find('.panel-body'));
                                    }

                                    newPanel.appendTo($jsTB.page.elements.monitorList);
                                }
                            }
                            break;
                    }
                },
            });
        }
    </script>

    <ol class="breadcrumb">
      <li class="active">Home</li>
    </ol>

    <div class="panel panel-info">
      <div class="panel-heading">Machine</div>

      <div class="panel-body">
        <!-- CPU -->
        <div class="media">
          <div class="media-left">
            <a href="#">
              <img class="media-object" src="img/cpu.png">
            </a>
          </div>

          <div class="media-body">
            <h4 class="media-heading">CPU</h4>
            <div id="cdbCPUInfo"></div>
          </div>
        </div>

        <!-- memory -->
        <div class="media">
          <div class="media-left">
            <a href="#">
              <img class="media-object" src="img/memory.png">
            </a>
          </div>

          <div class="media-body">
            <h4 class="media-heading">Memory</h4>
            <div id="cdbMemInfo"></div>
          </div>
        </div>
      </div>
    </div>
    
    <div class="panel panel-info">
      <div class="panel-heading">Monitors</div>

      <div class="panel-body" id="cdbMonitorList"></div>
    </div>
</asp:Content>