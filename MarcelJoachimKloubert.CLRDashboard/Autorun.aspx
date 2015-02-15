<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/main.Master" AutoEventWireup="true" CodeBehind="Autorun.aspx.cs" Inherits="MarcelJoachimKloubert.CLRDashboard.Autorun" %>

<%@ Import Namespace="System.Management" %>

<asp:Content ID="Content_Main" ContentPlaceHolderID="ContentPlaceHolder_Main" runat="server">
    <style type="text/css">
        .cdbAutorunEntryItem img {
            margin-right: 1em;
            width: 2em;
        }
    </style>

    <ol class="breadcrumb">
      <li><a href="Default.aspx">Home</a></li>
      <li class="active">Autorun</li>
    </ol>

    <div class="panel panel-info">
      <div class="panel-heading">Processes</div>

      <div class="panel-body">

    <%
        if (this.AutorunEntries.Length > 0)
        {
            for (var i = 0; i < this.AutorunEntries.Length; i++)
            {
                var entry = this.AutorunEntries[i];
                
                var anchor = string.Format("cdbAutorunEntry{0}", i);
                var elementId = string.Format("cdbAutorunEntryItem{0}", i);
                
                %>
                    <a name="<%= anchor%>"></a>
                    <div class="media cdbAutorunEntryItem" id="<%= elementId %>">
                        <div class="media-left">
                            <a href="#">
                                <img class="media-object" src="img/autostart.png">
                            </a>
                        </div>

                        <div class="media-body">
                            <h4 class="media-heading"><%= HttpUtility.HtmlEncode(entry.Caption) %></h4>
                            <%= HttpUtility.HtmlEncode(entry.Command) %>
                        </div>
                    </div>
                <%
            }
            
            
        }
        else
        {
            %>No entries found!<%
        }
    %>
      </div>
    </div>
</asp:Content>

