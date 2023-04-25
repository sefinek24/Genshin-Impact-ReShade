
#if !defined(CMACROS_FXH)
    #define CMACROS_FXH

    #define FP16_SMALLEST_SUBNORMAL float((1.0 / (1 << 14)) * (0.0 + (1.0 / (1 << 10))))

    #define BUFFER_SIZE_0 int2(BUFFER_WIDTH, BUFFER_HEIGHT)
    #define BUFFER_SIZE_1 int2(BUFFER_SIZE_0 >> 1)
    #define BUFFER_SIZE_2 int2(BUFFER_SIZE_0 >> 2)
    #define BUFFER_SIZE_3 int2(BUFFER_SIZE_0 >> 3)
    #define BUFFER_SIZE_4 int2(BUFFER_SIZE_0 >> 4)
    #define BUFFER_SIZE_5 int2(BUFFER_SIZE_0 >> 5)
    #define BUFFER_SIZE_6 int2(BUFFER_SIZE_0 >> 6)
    #define BUFFER_SIZE_7 int2(BUFFER_SIZE_0 >> 7)
    #define BUFFER_SIZE_8 int2(BUFFER_SIZE_0 >> 8)

    #define CREATE_OPTION(DATATYPE, NAME, CATEGORY, LABEL, TYPE, MAXIMUM, DEFAULT) \
        uniform DATATYPE NAME < \
            ui_category = CATEGORY; \
            ui_label = LABEL; \
            ui_type = TYPE; \
            ui_min = 0.0; \
            ui_max = MAXIMUM; \
        > = DEFAULT;

    #define CREATE_TEXTURE(TEXTURE_NAME, SIZE, FORMAT, LEVELS) \
        texture2D TEXTURE_NAME \
        { \
            Width = SIZE.x; \
            Height = SIZE.y; \
            Format = FORMAT; \
            MipLevels = LEVELS; \
        };

    #define CREATE_SAMPLER(SAMPLER_NAME, TEXTURE, FILTER, ADDRESS) \
        sampler2D SAMPLER_NAME \
        { \
            Texture = TEXTURE; \
            MagFilter = FILTER; \
            MinFilter = FILTER; \
            MipFilter = FILTER; \
            AddressU = ADDRESS; \
            AddressV = ADDRESS; \
        };

    #if BUFFER_COLOR_BIT_DEPTH == 8
        #define READ_SRGB TRUE
        #define WRITE_SRGB TRUE
    #else
        #define READ_SRGB FALSE
        #define WRITE_SRGB FALSE
    #endif

    #define CREATE_SRGB_SAMPLER(SAMPLER_NAME, TEXTURE, FILTER, ADDRESS) \
        sampler2D SAMPLER_NAME \
        { \
            Texture = TEXTURE; \
            MagFilter = FILTER; \
            MinFilter = FILTER; \
            MipFilter = FILTER; \
            AddressU = ADDRESS; \
            AddressV = ADDRESS; \
            SRGBTexture = READ_SRGB; \
        };
#endif
