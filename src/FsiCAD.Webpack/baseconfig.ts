import * as webpack from 'webpack';

export const BaseConfig: webpack.Configuration = {
    experiments: {
        outputModule: true,
    },
    output: {
        library: { type: 'module' },
        filename: '[name].js'
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                enforce: 'pre',
                use: ['source-map-loader'],
            },
        ],
    },
};
