/// <binding />
var gulp = require("gulp");
//var _msbuild = require("gulp-msbuild");
//var debug = require("gulp-debug");
//var foreach = require("gulp-foreach");
//var rename = require("gulp-rename");
//var newer = require("gulp-newer");
//var util = require("gulp-util");
var runSequence = require("run-sequence");
var nugetRestore = require("gulp-nuget-restore");
//var fs = require("fs");
//var yargs = require("yargs").argv;
//var path = require("path");

var config = require("./gulp-config.js")();

function defaultTask(cb) {
    //cb();

    config.runCleanBuilds = true;
    return runSequence(
        "Publish-All-Projects",
        cb
    );
}

module.exports.config = config;
module.exports.default = defaultTask;