import * as webpack from 'webpack';
import * as MiniCssExtractPlugin from 'mini-css-extract-plugin';
import * as CssMinimizerPlugin from 'css-minimizer-webpack-plugin';
import * as RemoveEmptyScriptsPlugin from 'webpack-remove-empty-scripts';

export const CssMinificationConfig: webpack.Configuration = {
    plugins: [
        new RemoveEmptyScriptsPlugin.default({}),
        new MiniCssExtractPlugin.default()
    ],

    module: {
        rules: [
            {
                test: /\.scss$/,
                use: [MiniCssExtractPlugin.loader, 'css-loader', 'sass-loader']
            },
        ],
    },

    optimization: {
        minimize: true,
        minimizer: [
            new CssMinimizerPlugin.default({
                include: /\.min\.css$/
            })
        ]
    },
};
