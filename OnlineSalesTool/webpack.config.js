'use strict';
const webpack = require('webpack');
const CleanWebpackPlugin = require('clean-webpack-plugin')
const { VueLoaderPlugin } = require('vue-loader')
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
module.exports = {
    entry: {
        app: './wwwroot/src/App/app.js'
    },
    output: {
        path: __dirname + "/wwwroot/dist/",
        filename: "[name].js",
        publicPath: '/dist/',
        chunkFilename: '[name].js',
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
                use: [
                    MiniCssExtractPlugin.loader,
                    {
                      loader: "css-loader",
                      query: {
                        importLoaders: 1
                      }
                    }
                  ]
            },
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            }
        ]
    },
    optimization: {
        splitChunks: {
            cacheGroups: {
                styles: {
                    name: 'app_styles',
                    test: module =>  module.nameForCondition &&
                        /\.(s?css|vue)$/.test(module.nameForCondition()) &&
                        !/^javascript/.test(module.type),
                    chunks: 'all',
                    enforce: true
                }
            }
        }
    },
    plugins: [
        //Ignore locales to reduce bundled size
        new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/), 
        new CleanWebpackPlugin(['wwwroot/dist'], []),
        new VueLoaderPlugin(),
        new MiniCssExtractPlugin()
    ],
    stats: {
        warnings: false
    }
};