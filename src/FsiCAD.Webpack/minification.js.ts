import * as webpack from 'webpack';
import * as TerserPlugin from 'terser-webpack-plugin';

export const JsMinificationConfig: webpack.Configuration = {
    output: {
        filename: '[name].min.js'
    },
    optimization: {
        minimizer: [
            new TerserPlugin.default(),
        ],
    }
};
