# Unity-Template-Project
Strucktur Project
- Startup 
- Auth Menu (Login, Register, Token)
- Lobby Menu (User Profile)

Feature
- MVP (Model View Presenter)
- UniTask
- DI (VContainer)

Service
- Navigation Service (Route panel and scene)
- API Service
- Addressable Asset
- Global UI
- Audio Service
  
[Template-Base-Flow.drawio](https://github.com/user-attachments/files/25179380/Template-Base-Flow.drawio)
<mxfile host="app.diagrams.net" agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/144.0.0.0 Safari/537.36" version="29.3.7">
  <diagram id="C5RBs43oDa-KdzZeNtuy" name="Page-1">
    <mxGraphModel dx="952" dy="873" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="1090" pageHeight="980" math="0" shadow="0">
      <root>
        <mxCell id="WIyWlLk6GJQsqaUBKTNV-0" />
        <mxCell id="WIyWlLk6GJQsqaUBKTNV-1" parent="WIyWlLk6GJQsqaUBKTNV-0" />
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-25" edge="1" parent="WIyWlLk6GJQsqaUBKTNV-1" source="vCJ9NhfWlQxAXHgEbB82-92" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;exitX=0.5;exitY=1;exitDx=0;exitDy=0;entryX=0.5;entryY=0;entryDx=0;entryDy=0;" target="vCJ9NhfWlQxAXHgEbB82-18" value="">
          <mxGeometry relative="1" as="geometry">
            <Array as="points">
              <mxPoint x="420" y="400" />
              <mxPoint x="250" y="400" />
            </Array>
            <mxPoint x="414" y="430" as="sourcePoint" />
          </mxGeometry>
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-45" connectable="0" parent="vCJ9NhfWlQxAXHgEbB82-25" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="Yes" vertex="1">
          <mxGeometry relative="1" x="0.05" as="geometry">
            <mxPoint as="offset" />
          </mxGeometry>
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-42" edge="1" parent="WIyWlLk6GJQsqaUBKTNV-1" source="vCJ9NhfWlQxAXHgEbB82-92" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;exitX=0.5;exitY=1;exitDx=0;exitDy=0;" target="vCJ9NhfWlQxAXHgEbB82-35" value="">
          <mxGeometry relative="1" as="geometry">
            <Array as="points">
              <mxPoint x="420" y="400" />
              <mxPoint x="580" y="400" />
            </Array>
            <mxPoint x="414" y="430" as="sourcePoint" />
          </mxGeometry>
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-44" connectable="0" parent="vCJ9NhfWlQxAXHgEbB82-42" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="Not Login / Token Expired" vertex="1">
          <mxGeometry relative="1" x="-0.825" y="-2" as="geometry">
            <mxPoint as="offset" />
          </mxGeometry>
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-46" connectable="0" parent="vCJ9NhfWlQxAXHgEbB82-42" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="No" vertex="1">
          <mxGeometry relative="1" x="0.2917" y="2" as="geometry">
            <mxPoint as="offset" />
          </mxGeometry>
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-80" edge="1" parent="WIyWlLk6GJQsqaUBKTNV-1" source="vCJ9NhfWlQxAXHgEbB82-3" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;entryX=0.5;entryY=0;entryDx=0;entryDy=0;" target="vCJ9NhfWlQxAXHgEbB82-92" value="">
          <mxGeometry relative="1" as="geometry">
            <mxPoint x="420" y="280" as="targetPoint" />
          </mxGeometry>
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-3" parent="WIyWlLk6GJQsqaUBKTNV-1" style="swimlane;" value="Startup" vertex="1">
          <mxGeometry height="190" width="200" x="320" y="50" as="geometry">
            <mxRectangle height="30" width="120" x="320" y="40" as="alternateBounds" />
          </mxGeometry>
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-10" parent="vCJ9NhfWlQxAXHgEbB82-3" style="whiteSpace=wrap;html=1;" value="Entry Point" vertex="1">
          <mxGeometry height="37" width="74" x="63" y="90" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-11" parent="vCJ9NhfWlQxAXHgEbB82-3" style="whiteSpace=wrap;html=1;" value="Loading UI" vertex="1">
          <mxGeometry height="37" width="74" x="63" y="140" as="geometry" />
        </mxCell>
        <mxCell id="Cxso395sfYLqAgv7VZVF-0" edge="1" parent="vCJ9NhfWlQxAXHgEbB82-3" source="vCJ9NhfWlQxAXHgEbB82-12" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;exitX=0.5;exitY=1;exitDx=0;exitDy=0;entryX=0.5;entryY=0;entryDx=0;entryDy=0;" target="vCJ9NhfWlQxAXHgEbB82-10">
          <mxGeometry relative="1" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-12" parent="vCJ9NhfWlQxAXHgEbB82-3" style="whiteSpace=wrap;html=1;gradientColor=none;dashed=1;" value="Startup" vertex="1">
          <mxGeometry height="37" width="74" x="63" y="40" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-18" parent="WIyWlLk6GJQsqaUBKTNV-1" style="swimlane;" value="Auth" vertex="1">
          <mxGeometry height="180" width="220" x="140" y="440" as="geometry">
            <mxRectangle height="30" width="120" x="320" y="40" as="alternateBounds" />
          </mxGeometry>
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-20" parent="vCJ9NhfWlQxAXHgEbB82-18" style="whiteSpace=wrap;html=1;" value="Auth View" vertex="1">
          <mxGeometry height="37" width="74" x="77" y="32" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-24" parent="vCJ9NhfWlQxAXHgEbB82-18" style="text;html=1;align=left;verticalAlign=middle;resizable=0;points=[];autosize=1;strokeColor=none;fillColor=none;" value="&lt;span style=&quot;&quot;&gt;Gl&lt;/span&gt;o&lt;span style=&quot;&quot;&gt;bal&amp;nbsp;&lt;/span&gt;&lt;div style=&quot;&quot;&gt;Scope&lt;/div&gt;" vertex="1">
          <mxGeometry height="40" width="60" x="3" y="23" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-98" parent="vCJ9NhfWlQxAXHgEbB82-18" style="whiteSpace=wrap;html=1;" value="Page Container&lt;div&gt;login, register, etc&lt;/div&gt;" vertex="1">
          <mxGeometry height="40" width="120" x="54" y="80" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-109" parent="vCJ9NhfWlQxAXHgEbB82-18" style="whiteSpace=wrap;html=1;" value="Auth Scope" vertex="1">
          <mxGeometry height="37" width="74" x="77" y="130" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-35" parent="WIyWlLk6GJQsqaUBKTNV-1" style="swimlane;" value="Lobby Game" vertex="1">
          <mxGeometry height="140" width="200" x="480" y="440" as="geometry">
            <mxRectangle height="30" width="120" x="320" y="40" as="alternateBounds" />
          </mxGeometry>
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-38" parent="vCJ9NhfWlQxAXHgEbB82-35" style="whiteSpace=wrap;html=1;" value="Lobby View" vertex="1">
          <mxGeometry height="37" width="74" x="63" y="40" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-41" parent="vCJ9NhfWlQxAXHgEbB82-35" style="text;html=1;align=left;verticalAlign=middle;resizable=0;points=[];autosize=1;strokeColor=none;fillColor=none;" value="Global&amp;nbsp;&lt;div&gt;Scope&lt;/div&gt;" vertex="1">
          <mxGeometry height="40" width="60" x="3" y="23" as="geometry" />
        </mxCell>
        <mxCell id="Cxso395sfYLqAgv7VZVF-2" parent="vCJ9NhfWlQxAXHgEbB82-35" style="whiteSpace=wrap;html=1;" value="Lobby Scope" vertex="1">
          <mxGeometry height="37" width="74" x="63" y="90" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-107" edge="1" parent="WIyWlLk6GJQsqaUBKTNV-1" source="vCJ9NhfWlQxAXHgEbB82-57" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;entryX=0.5;entryY=0;entryDx=0;entryDy=0;" target="vCJ9NhfWlQxAXHgEbB82-132" value="">
          <mxGeometry relative="1" as="geometry">
            <mxPoint x="170" y="270" as="targetPoint" />
          </mxGeometry>
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-57" parent="WIyWlLk6GJQsqaUBKTNV-1" style="swimlane;fontStyle=0;childLayout=stackLayout;horizontal=1;startSize=40;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="&lt;b&gt;Global Scope&lt;/b&gt;&lt;br&gt;(Dont Destroyable)" vertex="1">
          <mxGeometry height="140" width="140" x="100" y="70" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-63" parent="vCJ9NhfWlQxAXHgEbB82-57" style="text;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;rotatable=0;whiteSpace=wrap;html=1;" value="Game API Service" vertex="1">
          <mxGeometry height="20" width="140" y="40" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-59" parent="vCJ9NhfWlQxAXHgEbB82-57" style="text;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;rotatable=0;whiteSpace=wrap;html=1;" value="Navigation" vertex="1">
          <mxGeometry height="20" width="140" y="60" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-60" parent="vCJ9NhfWlQxAXHgEbB82-57" style="text;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;rotatable=0;whiteSpace=wrap;html=1;" value="Global UI Service" vertex="1">
          <mxGeometry height="20" width="140" y="80" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-58" parent="vCJ9NhfWlQxAXHgEbB82-57" style="text;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;rotatable=0;whiteSpace=wrap;html=1;" value="Audio Service" vertex="1">
          <mxGeometry height="20" width="140" y="100" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-110" parent="vCJ9NhfWlQxAXHgEbB82-57" style="text;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;rotatable=0;whiteSpace=wrap;html=1;" value="Asset Service" vertex="1">
          <mxGeometry height="20" width="140" y="120" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-91" parent="WIyWlLk6GJQsqaUBKTNV-1" style="text;html=1;align=left;verticalAlign=middle;resizable=0;points=[];autosize=1;strokeColor=none;fillColor=none;" value="&lt;b&gt;&lt;font style=&quot;font-size: 15px;&quot;&gt;Base View&amp;nbsp;&lt;/font&gt;&lt;/b&gt;&lt;b&gt;&lt;font style=&quot;font-size: 15px;&quot;&gt;&amp;nbsp;App&amp;nbsp;&lt;/font&gt;&lt;/b&gt;&lt;b&gt;&lt;font style=&quot;font-size: 15px;&quot;&gt;Flow&lt;/font&gt;&lt;/b&gt;" vertex="1">
          <mxGeometry height="30" width="170" x="10" y="10" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-95" edge="1" parent="WIyWlLk6GJQsqaUBKTNV-1" source="vCJ9NhfWlQxAXHgEbB82-92" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;" target="vCJ9NhfWlQxAXHgEbB82-94" value="">
          <mxGeometry relative="1" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-97" connectable="0" parent="vCJ9NhfWlQxAXHgEbB82-95" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="Service Error" vertex="1">
          <mxGeometry relative="1" x="-0.0308" y="-2" as="geometry">
            <mxPoint x="-3" y="-2" as="offset" />
          </mxGeometry>
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-92" parent="WIyWlLk6GJQsqaUBKTNV-1" style="rhombus;whiteSpace=wrap;html=1;" value="Condition" vertex="1">
          <mxGeometry height="80" width="80" x="380" y="270" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-94" parent="WIyWlLk6GJQsqaUBKTNV-1" style="whiteSpace=wrap;html=1;" value="&lt;b&gt;Page Error&lt;/b&gt;&lt;div&gt;&lt;br&gt;&lt;div&gt;(Maintenance/ Force Update, etc)&lt;/div&gt;&lt;/div&gt;" vertex="1">
          <mxGeometry height="80" width="180" x="590" y="270" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-132" parent="WIyWlLk6GJQsqaUBKTNV-1" style="swimlane;fontStyle=0;childLayout=stackLayout;horizontal=1;startSize=20;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="&lt;b&gt;Interface&lt;/b&gt;" vertex="1">
          <mxGeometry height="120" width="140" x="100" y="240" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-133" parent="vCJ9NhfWlQxAXHgEbB82-132" style="text;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;rotatable=0;whiteSpace=wrap;html=1;" value="IGameApiService" vertex="1">
          <mxGeometry height="20" width="140" y="20" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-134" parent="vCJ9NhfWlQxAXHgEbB82-132" style="text;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;rotatable=0;whiteSpace=wrap;html=1;" value="INavigationService" vertex="1">
          <mxGeometry height="20" width="140" y="40" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-135" parent="vCJ9NhfWlQxAXHgEbB82-132" style="text;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;rotatable=0;whiteSpace=wrap;html=1;" value="IGlobalUIService" vertex="1">
          <mxGeometry height="20" width="140" y="60" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-136" parent="vCJ9NhfWlQxAXHgEbB82-132" style="text;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;rotatable=0;whiteSpace=wrap;html=1;" value="Audio Service" vertex="1">
          <mxGeometry height="20" width="140" y="80" as="geometry" />
        </mxCell>
        <mxCell id="vCJ9NhfWlQxAXHgEbB82-137" parent="vCJ9NhfWlQxAXHgEbB82-132" style="text;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;rotatable=0;whiteSpace=wrap;html=1;" value="IAssetProvider" vertex="1">
          <mxGeometry height="20" width="140" y="100" as="geometry" />
        </mxCell>
        <mxCell id="Cxso395sfYLqAgv7VZVF-1" edge="1" parent="WIyWlLk6GJQsqaUBKTNV-1" source="vCJ9NhfWlQxAXHgEbB82-12" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;exitX=0;exitY=0.5;exitDx=0;exitDy=0;entryX=1;entryY=0.5;entryDx=0;entryDy=0;" target="vCJ9NhfWlQxAXHgEbB82-60">
          <mxGeometry relative="1" as="geometry" />
        </mxCell>
        <mxCell id="Cxso395sfYLqAgv7VZVF-4" parent="WIyWlLk6GJQsqaUBKTNV-1" style="swimlane;fontStyle=0;childLayout=stackLayout;horizontal=1;startSize=40;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="&lt;b&gt;Global Data&lt;/b&gt;&lt;br&gt;(Dont Destroyable)" vertex="1">
          <mxGeometry height="80" width="140" x="590" y="68.5" as="geometry" />
        </mxCell>
        <mxCell id="Cxso395sfYLqAgv7VZVF-5" parent="Cxso395sfYLqAgv7VZVF-4" style="text;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;rotatable=0;whiteSpace=wrap;html=1;" value="Panel Route Scriptable" vertex="1">
          <mxGeometry height="20" width="140" y="40" as="geometry" />
        </mxCell>
        <mxCell id="Cxso395sfYLqAgv7VZVF-6" parent="Cxso395sfYLqAgv7VZVF-4" style="text;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;rotatable=0;whiteSpace=wrap;html=1;" value="Audio Scriptable" vertex="1">
          <mxGeometry height="20" width="140" y="60" as="geometry" />
        </mxCell>
        <mxCell id="Cxso395sfYLqAgv7VZVF-10" edge="1" parent="WIyWlLk6GJQsqaUBKTNV-1" source="vCJ9NhfWlQxAXHgEbB82-12" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;exitX=1;exitY=0.5;exitDx=0;exitDy=0;" target="Cxso395sfYLqAgv7VZVF-4">
          <mxGeometry relative="1" as="geometry" />
        </mxCell>
      </root>
    </mxGraphModel>
  </diagram>
</mxfile>

Create an environment (.env) file to connect the game with your game API
Example : 
  API_BASE_URL=https://link/game
  API_BEARER=token
  API_SALT_PASSWORD=salt
