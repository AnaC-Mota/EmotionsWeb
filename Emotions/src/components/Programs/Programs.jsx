import './Programs.css'
import imagen_1 from '../../assets/imagen_1.png'
import imagen_2 from '../../assets/imagen_2.png'
import imagen_3 from '../../assets/imagen_3.png'

const Programs = () => {
  return (
    <div className='programs'>
      <div className="program">
        <img src={imagen_1} alt=""/>
        <div className="caption">
            <p><b>Registrar emoções</b> é como guardar pedaços de si,
             para que no futuro possamos revisitar nosso passado e 
              entender nossa jornada.</p>
        </div>
      </div>
      <div className="program">
        <img src={imagen_2} alt=""/>
        <div className="caption">
            <p>Gerar <b>relatórios e gráficos</b> do histórico é como transformar emoções
                 em traços visíveis, revelando a beleza dos nossos altos e baixos.</p>
        </div>
      </div>
      <div className="program">
        <img src={imagen_3} alt=""/>
        <div className="caption">
            <p><b>Artigos</b> que surgem dos relatórios são como respostas precisas, trazendo clareza, 
            insights valiosos e apoio estratégico no momento certo.</p>
        </div>
      </div>
    </div>
  )
}

export default Programs
