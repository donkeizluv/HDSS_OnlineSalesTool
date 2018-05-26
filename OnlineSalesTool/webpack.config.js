'use strict';
const webpack = require('webpack');
const CleanWebpackPlugin = require('clean-webpack-plugin')
const { VueLoaderPlugin } = require('vue-loader')

module.exports = {
    entry: {
        //Shared: './wwwroot/src/shared.js',
        home: './wwwroot/src/Home/app.js'
    },
    output: {
        path: __dirname + "/wwwroot/dist/",
        //filename: "[name]_[chunkhash].js"
        filename: "app.js",
        publicPath: '/dist/',
        chunkFilename: '[name].chunk.js',
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
        //Ignore locales to reduce bundled size
        new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/), 
        new CleanWebpackPlugin(['wwwroot/dist'], []),
        new VueLoaderPlugin()
    ],
    stats: {
        warnings: false
    }
};