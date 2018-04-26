'use strict';
const webpack = require('webpack');
const CleanWebpackPlugin = require('clean-webpack-plugin')
//const HardSourceWebpackPlugin = require('hard-source-webpack-plugin');

module.exports = {
    entry: {
        //Shared: './wwwroot/src/shared.js',
        Home: './wwwroot/src/Home/app.js'
    },
    output: {
        path: __dirname + "/wwwroot/dist/",
        //filename: "[name]_[chunkhash].js"
        filename: "app.js"
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /(node_modules)/,
                use: {
                    loader: 'babel-loader?cacheDirectory'
                }
            },
            {
                test: /\.css$/,
                loader: "style-loader!css-loader"
            },
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            }
        ]
    },
    plugins: [
        //Very slow
        //new webpack.optimize.UglifyJsPlugin({
        //    output: {
        //        comments: false //No comments
        //    }
        //}),
        new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/), //Ignore locales to reduce bundled size
        new CleanWebpackPlugin(['wwwroot/dist'], []),
        //new HardSourceWebpackPlugin()
    ],
    stats: {
        warnings: false
    }
};