var sqloogle;

$(function () {

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(window.location.href);
        if (results == null)
            return "";
        else
            return decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    function getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    function sqlResultModel(data) {
        var self = this;

        self.id = data.Id;
        self.server = data.Server;
        self.database = data.Database;
        self.schema = data.Schema;
        self.name = data.Name;
        self.type = data.Type;
        self.modified = data.ModifyDateFormatted;
        self.use = data.Use;
        self.lastUsed = data.LastUsedDateFormatted;
        self.url = data.Url;
        self.selected = ko.observable(false);
        self.iconStyle = ko.observable("");

        self.toggleIconStyle = function () {
            if (!self.selected()) {
                var a = "";
                var b = "background-image: url(" + window.settings.content + "images/icons_hover.png)";
                self.iconStyle(self.iconStyle() === a ? b : a);
            }
        };

        var turnUnderscoresIntoBorders = function (value) {
            if (value) {
                return value.replace(/_/g, '<span class="dim">  </span>');
            }
            return "";
        };

        var isMultiple = function (value) {
            return value.indexOf("|") > 0;
        };

        var formatMultiple = function (value, pluralName) {
            return '<span title="' + value.replace(/\|/, " | ") + '">(' + (value.split("|").length) + ' ' + pluralName + ')</span>';
        };

        self.useFormatted = function () {
            var use = self.use + ''; // coerce to string
            if (use == 0) {
                return "";
            } else if (use < 1000) {
                return use; // return the same number
            } else if (use < 1000000) { // divide and format
                return (use / 1000).toFixed(use % 1000 != 0) + 'K';
            } else { // divide and format
                return (use / 1000000).toFixed(use % 1000000 != 0) + 'M';
            }
        };

        self.databaseFormatted = function () {
            var database = self.database;
            if (isMultiple(database)) {
                return formatMultiple(database, "databases");
            }
            return turnUnderscoresIntoBorders(database);
        };

        self.serverFormatted = function () {
            if (isMultiple(self.server)) {
                return formatMultiple(self.server, "servers");
            }
            return self.server;
        };

        self.nameFormatted = function () {
            return turnUnderscoresIntoBorders(self.name);
        };

        self.sqlFind = function (success) {
            var url = window.settings.api + 'Sql/Find?';
            var params = { id: self.id };
            $.getJSON(url, params, function (result) {
                success(result);
            });
        };

        self.view = function () {
            self.sqlFind(function (result) {
                if (result.success) {
                    var sql = result.searchresults[0].SqlScript;
                    var formattedSql = prettyPrintOne(sql, 'sql');
                    $('pre#sqlText').html(formattedSql);
                    $.colorbox({
                        fixed: true,
                        inline: true,
                        href: "div#sqlView",
                        width: Math.round(($(window).width() * .8)) + 'px',
                        height: Math.round(($(window).height() * .8)) + 'px',
                        onOpen: function() { $('div#sqlView').show(); },
                        onCleanup: function() { $('div#sqlView').hide(); }
                    });
                } else {
                    alert(result.message);
                }
            });
        };

    }

    function miaResultModel(data) {
        var self = this;

        self.id = data.Id;
        self.server = data.Server;
        self.database = data.Database;
        self.schema = data.Schema;
        self.type = data.Type;
        self.name = data.Name;
        self.equality = data.Equality;
        self.inequality = data.Inequality;
        self.included = data.Included;
        self.score = data.Score;
        self.sqlQuery = data.SqlQuery;
        self.sqlIndexQuery = data.SqlIndexQuery;
        self.createIndexSql = data.CreateIndexSql;
        self.columnSummary = data.ColumnSummary;
        self.iconStyle = ko.observable("");

        self.toggleIconStyle = function () {
            var a = "";
            var b = "background-image: url(" + window.settings.content + "images/icons_hover.png)";
            self.iconStyle(self.iconStyle() === a ? b : a);
        };

        self.viewCreateIndexSql = function () {
            var formattedSql = prettyPrintOne(self.createIndexSql, 'sql');
            var html = '<pre class="lang-sql prettyprint linenums">' + formattedSql + '</pre>';
            var width = Math.round(($(window).width() * .8)) + 'px';
            var height = Math.round(($(window).height() * .8)) + 'px';
            $.colorbox({ fixed: true, html: html, width: width, height: height });
        };

    }

    function sqloogleModel() {
        var self = this;
        var empty = "";

        //data
        self.mode = ko.observable(getParameterByName("m") === "" ? "sql" : getParameterByName("m"));
        self.query = ko.observable(getParameterByName("q"));
        self.lastMiaQuery = empty;
        self.lastSqlQuery = empty;
        self.directLink = ko.observable(empty);
        self.error = ko.observable(empty);
        self.isSubsetOfMia = ko.observable(false);
        self.info = empty;

        self.sqlResults = ko.observableArray([]);
        self.miaResults = ko.observableArray([]);
        self.compareResults = ko.observableArray([]);
        self.comparing = ko.observable(false);
        self.loading = ko.observable(false);

        self.sqlMode = ko.computed(function () { return self.mode() === 'sql'; });
        self.miaMode = ko.computed(function () { return self.mode() === 'mia'; });

        self.isComparable = ko.computed(function () {
            return self.compareResults().length > 1;
        });

        self.toggleMode = function () {

            self.stopComparing();
            self.compareResults([]);
            self.isSubsetOfMia(false);

            if (self.miaMode()) {
                self.query(self.lastSqlQuery);
                self.sqlSearch(self.lastSqlQuery);
            } else {
                self.query(self.lastMiaQuery);
                self.miaSearch(self.lastMiaQuery);
            }

            $('.q').focus();
        };

        self.querySqlCauses = function (result) {
            self.query(empty);
            self.info = "Causes for missing index on " + result.name + ".";
            self.isSubsetOfMia(true);
            self.sqlSearch(result.sqlQuery);
        };

        self.queryExistingIndexes = function (result) {
            self.query(empty);
            self.info = "Existing indexes on " + result.name + ".";
            self.isSubsetOfMia(true);
            self.sqlSearch(result.sqlIndexQuery);
        };

        self.searchOnEnter = function (result, event) {
            if (event.which != null && event.which == 13) {
                var query = $('.q').val();
                if (self.sqlMode()) {
                    self.lastSqlQuery = query;
                    self.sqlSearch(query);
                } else {
                    self.lastMiaQuery = query;
                    self.miaSearch(query);
                }
                return false;
            } else {
                return true;
            }
        };

        self.selectedChange = function (searchResult, event) {
            var results = self.compareResults;
            var isSelected = $('#' + searchResult.id).is(":checked");
            if (isSelected) {
                switch (results().length) {
                    case 0:
                        results.push(searchResult);
                        break;
                    case 1:
                        results.unshift(searchResult);
                        break;
                    case 2:
                        results.pop().selected(false);
                        results.unshift(searchResult);
                        break;
                    default:
                }
            } else {
                results.remove(searchResult);
            }
        };

        self.sqlSearch = function (query) {
            self.mode('sql');
            if (query === empty) {
                self.sqlResults([]);
                self.error(empty);
            } else {
                self.loading(true);
                self.error(empty);
                self.directLink(empty);
                var url = window.settings.api + 'Sql/Search?';
                var random = getRandomInt(10000, 99999);
                var param = { 'q': query, 'r': random };
                $.getJSON(url, param, function (response) {
                    if (response) {
                        if (response.success) {
                            var results = $.map(response.searchresults, function (searchResult) { return new sqlResultModel(searchResult); });
                            if (results.length > 0) {
                                self.sqlResults(results);
                            } else {
                                self.sqlResults([]);
                                self.error("No Matching Sql");
                            }
                            var link = window.location.protocol + '//' + window.location.hostname + window.location.pathname + '?q=' + encodeURIComponent(query) + '&m=' + self.mode();
                            self.directLink(link);
                            self.loading(false);
                        } else {
                            self.sqlResults([]);
                            self.error(response.message);
                            self.loading(false);
                        }
                    }
                    ;
                });
            }
        };

        self.miaSearch = function (query) {
            self.mode('mia');
            self.error(empty);
            self.directLink(empty);
            self.loading(true);
            var url = window.settings.api + 'Mia/Search?';
            var random = getRandomInt(10000, 99999);
            var param = { 'q': query, 'r': random };
            $.getJSON(url, param, function (response) {
                if (response) {
                    if (response.success) {
                        var results = $.map(response.searchresults, function (searchResult) { return new miaResultModel(searchResult); });
                        if (results.length > 0) {
                            self.miaResults(results);
                        } else {
                            self.miaResults([]);
                            self.error("No Matching Mia");
                        }
                        var link = window.location.protocol + '//' + window.location.hostname + window.location.pathname + '?q=' + encodeURIComponent(query) + '&m=' + self.mode();
                        self.directLink(link);
                        self.loading(false);
                    } else {
                        self.miaResults([]);
                        self.error(response.message);
                        self.loading(false);
                    }
                }
                ;
            });
        };

        self.compare = function () {

            $('#compare').mergely({
                autoresize: true,
                autoupdate: true,
                width: 'auto',
                height: 'auto',
                ignorews: true,
                sidebar: false,
                cmsettings: { mode: 'text/x-sql', readOnly: true, lineNumbers: true, lineWrapping: true },
                lhs: function (setValue) { setValue(empty); },
                rhs: function (setValue) { setValue(empty); }
            });

            self.compareResults()[0].sqlFind(function (response) {
                $('#compare').mergely('lhs', response.searchresults[0].SqlScript);
            });

            self.compareResults()[1].sqlFind(function (response) {
                $('#compare').mergely('rhs', response.searchresults[0].SqlScript);
            });

            self.comparing(true);
        };

        self.stopComparing = function () {
            self.comparing(false);
        };

        if (self.query() != empty) {
            if (self.sqlMode()) {
                self.sqlSearch(self.query());
            } else {
                self.miaSearch(self.query());
            }
        }

    }

    sqloogle = new sqloogleModel();

    $(document).ready(function () {
        ko.applyBindings(sqloogle);
    });

});

