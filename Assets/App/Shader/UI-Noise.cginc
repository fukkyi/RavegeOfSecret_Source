struct appdata_t
{
    half4 vertex : POSITION;
    half2 uv : TEXCOORD0;
    fixed4 color : COLOR;
};

struct v2f
{
    half4 vertex : POSITION;
    half2 uv : TEXCOORD0;
    fixed4 color : COLOR;
};

sampler2D _MainTex;
half4 _MainTex_ST;
fixed4 _Color;

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.color = v.color * _Color;
    return o;
}

half random(half2 p, half seed)
{
    return frac(sin(dot(p, fixed2(12.9898, 78.233)) + seed) * 43758.5453);
}

half4 sandstorm(v2f i) : SV_Target
{
    half rand = random(half2(i.uv.x, i.uv.y), _Time);
    return half4(rand * i.color.r, rand * i.color.g, rand * i.color.b, i.color.a);
}